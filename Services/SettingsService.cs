using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace PhaserIDE.Services
{
    public static class SettingsService
    {
        private static readonly bool DEFAULT_THEME = false;
        private static readonly double DEFAULT_EDITOR_FONT_SIZE = 14.0;

        private static bool _isDarkTheme = false;
        private static bool _showLineNumbers = false;
        private static double _fontSize = DEFAULT_EDITOR_FONT_SIZE;

        public static event EventHandler<string> SettingsChanged;

        public static void InitializeSettings()
        {
            SetTheme(LoadThemePreference());
            EditorFontSize = LoadFontSizeSetting();
            ShowLineNumbers = LoadShowLineNumbers();
        }

        public static bool IsDarkTheme
        {
            get => _isDarkTheme;
            internal set
            {
                _isDarkTheme = value;
                SettingsChanged?.Invoke(null, nameof(IsDarkTheme));
            }
        }

        public static double EditorFontSize 
        { 
            get => _fontSize; 
            internal set 
            { 
                _fontSize = value;
                SettingsChanged?.Invoke(null, nameof(EditorFontSize));
            } 
        }

        public static bool ShowLineNumbers { 
            get => Settings.Default.ShowLineNumbers; 
            internal set {
                _showLineNumbers = value;
                SettingsChanged?.Invoke(null, nameof(ShowLineNumbers));
            } 
        }

        private static bool LoadShowLineNumbers()
        {
            return Settings.Default.ShowLineNumbers;
        }

        public static void SaveShowLineNumbers(bool show)
        {
            ShowLineNumbers = show;
            Settings.Default.ShowLineNumbers = show;
            Settings.Default.Save();
        }

        public static void SetTheme(bool isDark)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
            paletteHelper.SetTheme(theme);
            IsDarkTheme = isDark;

            // In Settings speichern (optional)
            SaveThemePreference(isDark);
        }

        private static void SaveThemePreference(bool isDark)
        {
            try
            {
                Settings.Default.IsDarkTheme = isDark;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme speichern fehlgeschlagen: {ex.Message}");
            }
        }

        private static bool LoadThemePreference()
        {
            try
            {
                return Settings.Default.IsDarkTheme;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme laden fehlgeschlagen: {ex.Message}");
                return DEFAULT_THEME;
            }
        }

        private static double LoadFontSizeSetting()
        {
            try
            {
                return Settings.Default.EditorFontSize;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Schriftgröße laden fehlgeschlagen: {ex.Message}");
                return DEFAULT_EDITOR_FONT_SIZE;
            }
        }

        public static bool SaveFontSizeSetting(double value)
        {
            try
            {
                EditorFontSize = value;
                Settings.Default.EditorFontSize = value;
                Settings.Default.Save();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Schriftgröße speichern fehlgeschlagen: {ex.Message}");
                return false;
            }
        }

        public static void SetPrimaryColor(Color color)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(color);
            paletteHelper.SetTheme(theme);
        }

        public static void SetSecondaryColor(Color color)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetSecondaryColor(color);
            paletteHelper.SetTheme(theme);
        }
    }
}
