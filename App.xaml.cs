using System.Windows;
using PhaserIDE.Services;

namespace PhaserIDE
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Theme Service initialisieren
            SettingsService.InitializeSettings();

            base.OnStartup(e);
        }
    }
}