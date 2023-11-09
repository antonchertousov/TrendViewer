using System.Diagnostics.CodeAnalysis;
using System.Windows;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using TrendViewer.DataProcessing;
using TrendViewer.Helpers;
using TrendViewer.Models;
using TrendViewer.ViewModels;
using TrendViewer.Views;

namespace TrendViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        /// <summary>
        /// Service provider for the DI container implementation
        /// </summary>
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            // Configure DI container
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Configure logging
            log4net.Config.XmlConfigurator.Configure();
            log4net.Util.SystemInfo.NullText = string.Empty;
            log.Info("============= Logging started =============");
        }
        
        private void ConfigureServices(ServiceCollection services)
        {
            // Register services 
            services.AddTransient<IOpenFileDialogHelper, OpenFileDialogHelper>();
            services.AddTransient<IDataReader, DataReader>();
            services.AddTransient<IDataProcessor, DataProcessor>();
            // Register models
            services.AddSingleton<IMainWindowModel, MainWindowModel>();
            services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}