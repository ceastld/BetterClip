using BetterClip.View.Pages;
using BetterClip.View.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace BetterClip.Service
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService(IServiceProvider serviceProvider, ConfigService configService) : IHostedService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ConfigService _configService = configService;
        private INavigationWindow _navigationWindow = default!;

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public static Type NavagatePageType { get; set; } = typeof(DashboardPage);

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                _navigationWindow = (
                    _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
                )!;

                ApplicationThemeManager.Apply(_configService.GlobalConfig.ApplicationTheme);

                _navigationWindow!.ShowWindow();

                _navigationWindow.Navigate(NavagatePageType);
            }

            await Task.CompletedTask;
        }
    }
}
