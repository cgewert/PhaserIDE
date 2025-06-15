using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhaserIDE.Services;
using PhaserIDE.ViewModels;
using System.Windows;

namespace PhaserIDE
{
    public partial class App : Application
    {
        private IHost _host;
        public IServiceProvider Services => _host.Services;

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();

            _host.Start();

            ViewModelService.Init(_host.Services);
            SettingsService.InitializeSettings();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // ViewModels
            services.AddSingleton<SingletonConsoleViewModel>();
            services.AddSingleton<NpmService>();
            services.AddTransient<ConsoleViewModel>();
            services.AddTransient<TemplatePlaceholderAnalyzer>();

            // Views
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
                await _host.StopAsync();

            base.OnExit(e);
        }
    }
}