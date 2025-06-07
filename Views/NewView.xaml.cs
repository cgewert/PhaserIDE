using MaterialDesignThemes.Wpf;
using PhaserIDE.Services;
using PhaserIDE.Services.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace PhaserIDE.Views
{
    /// <summary>
    /// Interaktionslogik für HomeView.xaml
    /// </summary>
    public partial class NewView : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public static readonly string PROCESS_NAME = "phaser.exe";
        public static readonly Brush ErrorBrush = Brushes.Red;

        private ITemplatePlaceholderAnalyzer _templatePlaceholderAnalyzer;


        public NewView()
        {
            InitializeComponent();
            DataContext = this;
            _templatePlaceholderAnalyzer = ViewModelService.InstanceOf<TemplatePlaceholderAnalyzer>();
            Loaded += NewView_Loaded;
        }

        private async void NewView_Loaded(object sender, RoutedEventArgs e)
        {
            var result = await _templatePlaceholderAnalyzer.AnalyzeAsync(@"E:\Programmieren\phaser-gen-cli\src\templates");

            foreach (var placeholder in result.Keys)
            {
                var textBox = new System.Windows.Controls.TextBox
                {
                    Margin = new Thickness(0, 8, 0, 8),
                    MinWidth = 300
                };

                HintAssist.SetHint(textBox, placeholder);

                PlaceholderPanel.Children.Add(textBox);
            }
        }

        private bool _hasErrors = false;
        private bool _projectCreated = false;

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
            var projectName = ProjectNameBox.Text?.Trim();
            var targetFolder = TargetFolderBox.Text?.Trim();
            var phaserVersion = (PhaserVersionBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            StatusBlock.Text = string.Empty;
            PhaserConsole.Clear();
            HasErrors = false;

            if (!IsValidPath(targetFolder) || string.IsNullOrEmpty(projectName))
            {
                HasErrors = true;
                // Override all other styles
                StatusBlock.Foreground = (Brush)Resources["ErrorBrush"];
                StatusBlock.Text = "Please fill out all fields correctly!";
                return;
            }

            StatusBlock.Text = "Creating project...";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = PROCESS_NAME,
                    Arguments = $"init --name \"{projectName}\" --output \"{targetFolder}\"",
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
                    }
                    else
                    {
                        HasErrors = true;
                        StatusBlock.Text = $"Error: {process.ExitCode}";
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
