using MaterialDesignThemes.Wpf;
using PhaserIDE.Services;
using PhaserIDE.Services.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace PhaserIDE.Views
{
    /// <summary>
    /// Interaktionslogik für NewView.xaml
    /// </summary>
    public partial class NewView : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public static readonly string PROCESS_NAME = "phaser.exe";
        public static readonly Brush ErrorBrush = Brushes.Red;

        private ITemplatePlaceholderAnalyzer _templatePlaceholderAnalyzer;
        private NpmService _npmService;

        public NewView()
        {
            InitializeComponent();
            DataContext = this;
            _isCreateButtonEnabled = true;
            _templatePlaceholderAnalyzer = ViewModelService.InstanceOf<TemplatePlaceholderAnalyzer>();
            _npmService = ViewModelService.InstanceOf<NpmService>();
            Loaded += NewView_Loaded;
        }

        public bool IsCreateButtonEnabled
        {
            get => _isCreateButtonEnabled;
            set
            {
                _isCreateButtonEnabled = value;
                OnPropertyChanged(nameof(IsCreateButtonEnabled));
            }
        }

        public string CreateButtonTooltip
        {
            get => _isCreateButtonEnabled ? "" : "Creating project...please wait!";
        }

        private async void NewView_Loaded(object sender, RoutedEventArgs e)
        {
            var versions = await _npmService.GetPackageVersionsAsync("phaser");
            if (versions.Count > 0)
            {
                // Add the latest version as the first item in the ComboBox
                PhaserVersionBox.ItemsSource = versions;
                PhaserVersionBox.SelectedIndex = 0;
            }
            else
            {
                // If no versions are available, disable the ComboBox
                PhaserVersionBox.IsEnabled = false;
            }

            // TODO: Use cli install folder variable here, instead of hard coded path
            var result = await _templatePlaceholderAnalyzer.AnalyzeAsync(@"E:\Programmieren\phaser-gen-cli\src\templates");

            foreach (var placeholder in result.Keys)
            {
                if(placeholder == "PHASER_VERSION")
                    continue; // Skip the PHASER_VERSION placeholder, as it is handled separately

                var fileList = result[placeholder].Select(f => {
                    return (Path.GetFileName(f) + "; ").Replace(".template", "");
                });

                var label = new System.Windows.Controls.Label
                {
                    Content = placeholder,
                    Margin = new Thickness(0, 8, 0, 0)
                };
                var textBox = new System.Windows.Controls.TextBox
                {
                    Margin = new Thickness(0, 8, 0, 8),
                    MinWidth = 300,
                    Tag = placeholder
                };

                HintAssist.SetHint(textBox, placeholder);

                PlaceholderGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Grid.SetColumn(label, 0);
                Grid.SetColumn(textBox, 1);
                Grid.SetRow(label, PlaceholderGrid.RowDefinitions.Count - 1);
                Grid.SetRow(textBox, PlaceholderGrid.RowDefinitions.Count - 1);
                
                PlaceholderGrid.Children.Add(textBox);
                PlaceholderGrid.Children.Add(label);
            }
        }

        private bool _hasErrors = false;
        private bool _projectCreated = false;
        private bool _isCreateButtonEnabled;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged(nameof(HasErrors));
            }
        }
        public bool ProjectCreatedSuccessfully
        {
            get => _projectCreated;
            set
            {
                _projectCreated = value;
                OnPropertyChanged(nameof(ProjectCreatedSuccessfully));
            }
        }

        private void OnBrowseFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose target folder:";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TargetFolderBox.Text = dialog.SelectedPath;
                }
            }
        }

        private async void OnCreateProject(object sender, RoutedEventArgs e)
        {
            IsCreateButtonEnabled = false;

            string ConfigFileName = "project.config.json";
            var projectName = ProjectNameBox.Text?.Trim();
            var targetFolder = TargetFolderBox.Text?.Trim();
            var phaserVersion = PhaserVersionBox.SelectedItem.ToString();
            var placeholderValues = new Dictionary<string, string>();

            if (!IsValidPath(targetFolder) || string.IsNullOrEmpty(projectName))
            {
                HasErrors = true;
                // Override all other styles
                StatusBlock.Foreground = (Brush)Resources["ErrorBrush"];
                StatusBlock.Text = "Please fill out all fields correctly!";
                IsCreateButtonEnabled = true;
                return;
            }

            if (!CheckIfProjectFolderIsEmpty(Path.Combine(targetFolder!, projectName)))
            {
                HasErrors = true;
                // Override all other styles
                StatusBlock.Foreground = (Brush)Resources["ErrorBrush"];
                StatusBlock.Text = "The project folder is not empty! Aborting generation.";
                IsCreateButtonEnabled = true;
                return;
            }

            foreach (var child in PlaceholderGrid.Children)
            {
                if (child is System.Windows.Controls.TextBox textBox && textBox.Tag is string placeholderKey)
                {
                    var text = textBox.Text.Trim();
                    if (!string.IsNullOrEmpty(text))
                        placeholderValues[placeholderKey] = text;
                }
            }
            // Add the PHASER_VERSION placeholder
            if (!string.IsNullOrEmpty(phaserVersion))
            {
                placeholderValues["PHASER_VERSION"] = phaserVersion;
            }
            else
            {
                HasErrors = true;
                StatusBlock.Foreground = (Brush)Resources["ErrorBrush"];
                StatusBlock.Text = "Please select a Phaser version!";
                IsCreateButtonEnabled = true;
                return;
            }

            var configPath = Path.Combine(targetFolder!, ConfigFileName);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(placeholderValues, jsonOptions);

            try
            {
                PhaserConsole.AddLog("Writing project configuration file to : \n" + configPath);
                await File.WriteAllTextAsync(configPath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error writing config file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            StatusBlock.Text = string.Empty;
            
            HasErrors = false;

            PhaserConsole.Clear();
            StatusBlock.Text = "Creating project...";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = PROCESS_NAME,
                    Arguments = $"init --name \"{projectName}\" --output \"{targetFolder}\" -c \"{configPath}\"",
                    WorkingDirectory = targetFolder,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                using (var process = new Process { StartInfo = psi, EnableRaisingEvents = true })
                {
                    // Handle stdout and stderr asynchronously
                    process.OutputDataReceived += (s, ea) =>
                    {
                        if (ea.Data != null)
                        {
                            Dispatcher.Invoke(() => PhaserConsole.AddLog(ea.Data));
                        }
                    };
                    process.ErrorDataReceived += (s, ea) =>
                    {
                        if (ea.Data != null)
                        {
                            // Append an error message with a different color
                            Dispatcher.Invoke(() => PhaserConsole.AddLog(ea.Data, true));
                        }
                    };
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode == 0)
                    {
                        HasErrors = false;
                        StatusBlock.Foreground = (Brush)Resources["SuccessBrush"];
                        StatusBlock.Text = $"Project '{projectName}' created successfully!";
                        File.Delete(configPath);
                        IsCreateButtonEnabled = true;
                    }
                    else
                    {
                        HasErrors = true;
                        StatusBlock.Text = $"Error: {process.ExitCode}";
                        IsCreateButtonEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                HasErrors = true;
                StatusBlock.Text = $"Error while creating project: {ex.Message}";
            }
            finally
            {
                StatusBlock.Foreground = HasErrors
                    ? (Brush)Resources["ErrorBrush"]
                    : new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        // TODO: Async ???
        private bool CheckIfProjectFolderIsEmpty(string path)
        {
            if (!Directory.Exists(path))
                return true;

            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private bool IsValidPath(string? path)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            return !string.IsNullOrWhiteSpace(path) && !path.Any(c => invalidChars.Contains(c));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
