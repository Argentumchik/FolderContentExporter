using FolderContentExporter;
using FolderContentExporter.Interfaces;
using FolderContentExporter.Services;
using FolderContentExporter.View;
using FolderContentExporter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FolderContentExporter.Core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<IFolderDialogService, FolderDialogService>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<IFileExportService, FileExportService>();
            services.AddSingleton<IErrorMapper, ErrorMapper>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<ExportDialogViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<ExportDialogWindow>();

            //Fabric
            services.AddSingleton<Func<ExportDialogWindow>>(provider =>
            {
                return () => provider.GetRequiredService<ExportDialogWindow>();
            });
        }
    }

}
