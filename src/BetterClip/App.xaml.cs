using System.IO;
using System.Reflection;
using System.Windows.Threading;
using BetterClip.Service;
using BetterClip.ViewModel.Pages;
using BetterClip.ViewModel.Windows;
using BetterClip.View.Pages;
using BetterClip.View.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using BetterClip.Service.Interface;
using Serilog;
using Microsoft.Extensions.Logging;
using BetterClip.ViewModel.Common;
using System.Diagnostics;
using BetterClip.Core.Config;
using Wpf.Ui.Appearance;
using Serilog.Sinks.RichTextBox.Abstraction;
using Serilog.Events;
using Serilog.Sinks.RichTextBox.Themes;

namespace BetterClip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(c =>
    {
        var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? string.Empty);
        c.SetBasePath(basePath!);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ConfigService>();

        var logFolder = Global.LogFolder;
        Directory.CreateDirectory(logFolder);
        var loggerConfiguration = new LoggerConfiguration()
            .WriteTo.File(path: Global.LogFile, outputTemplate: "[{Timestamp:HH:mm:ss.fff}] [{Level:u3}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}", rollingInterval: RollingInterval.Day);
        var richTextBox = new RichTextBoxImpl();
        services.AddSingleton<IRichTextBox>(richTextBox);
        loggerConfiguration.WriteTo.RichTextBox(richTextBox,
            LogEventLevel.Information,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
            theme: RichTextBoxConsoleThemes.Colored);
        Log.Logger = loggerConfiguration.CreateLogger();
        services.AddLogging(c => c.AddSerilog());

        services.AddSingleton<IMetadataService, MetaDataService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();

        services.AddHostedService<ApplicationHostService>();
        ApplicationHostService.NavagatePageType = typeof(FavorPage);

        // Page resolver service
        services.AddSingleton<IPageService, PageService>();

        // Theme manipulation
        services.AddSingleton<IThemeService, ThemeService>();

        // TaskBar manipulation
        services.AddSingleton<ITaskBarService, TaskBarService>();

        // Service containing navigation, same as INavigationWindow... but without window
        services.AddSingleton<INavigationService, NavigationService>();

        // Clip
        services.AddSingleton<IClipDataService, ClipDataService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddTransient<SearchHints>();
        services.AddSingleton<IFavorDataService, FavorDataService>();

        // Main window with navigation
        services.AddSingleton<INavigationWindow, MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<DashboardPage>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<DataPage>();
        services.AddSingleton<DataViewModel>();
        services.AddSingleton<SettingsPage>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<ClipPage>();
        services.AddSingleton<ClipViewModel>();
        services.AddSingleton<FavorPage>();
        services.AddSingleton<FavorViewModel>();
        services.AddSingleton<LoggingPage>();
        services.AddSingleton<LoggingViewModel>();

        // 添加页面的方法：需要同时注册页面和对应的 ViewModel 以及所有依赖项。若未正确注册，页面可能无法打开且不会报错。

    }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>() where T : class
        {
            return _host.Services.GetService<T>()
                ?? ActivatorUtilities.CreateInstance<T>(_host.Services);
        }

        public static ILogger<T> GetLogger<T>()
        {
            return _host.Services.GetService<ILogger<T>>()!;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // see: https://stackoverflow.com/questions/65739383/dispatcherscheduler-missing-from-system-reactive-5-0
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            _host.Start();
            var clipboardService = GetService<IClipboardService>();
            clipboardService.MonitorOn();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        public static void ReStart()
        {
            string appPath = Environment.ProcessPath ?? Global.AppPath;
            Process.Start(appPath);
            Current.Shutdown();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
            //e.Handled = true;
        }
    }
}
