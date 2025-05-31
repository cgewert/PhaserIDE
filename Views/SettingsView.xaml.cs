using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
using PhaserIDE.Services;

namespace PhaserIDE.Views
{
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;

            SettingsService.SettingsChanged += OnSettingsChanged;

            IsDarkTheme = SettingsService.IsDarkTheme;
            EditorFontSize = SettingsService.EditorFontSize;
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged();
            }
        }

        private double _editorFontSize;
        public double EditorFontSize
        {
            get => _editorFontSize;
            set 
            { 
                _editorFontSize = value;
                OnPropertyChanged();
            }
        }

        public bool ShowLineNumbers
        {
            get => SettingsService.ShowLineNumbers;
            set
            {
                SettingsService.SaveShowLineNumbers(value);
                OnPropertyChanged();
            }
        }

        private void EditorFontSize_Changed(object sender, RoutedEventArgs e)
        {
            var newSize = FontSizeSlider.Value;
            EditorFontSize = newSize;
            SettingsService.SaveFontSizeSetting(newSize);
        }

        private void ShowLineNumbers_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowLineNumbers = false;
        }

        private void ShowLineNumbers_Checked(object sender, RoutedEventArgs e)
        {
            ShowLineNumbers = true;
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            SettingsService.SetTheme(true);
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsService.SetTheme(false);
        }

        private void OnSettingsChanged(object sender, string settingsName)
        {
            switch(settingsName)
            {
                case "IsDarkTheme":
                    IsDarkTheme = SettingsService.IsDarkTheme;
                    break;
                case "FontSize":
                    EditorFontSize = SettingsService.EditorFontSize;
                    break;
            }
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}