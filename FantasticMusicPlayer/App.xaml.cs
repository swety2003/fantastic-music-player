using FantasticMusicPlayer.Contracts;
using FantasticMusicPlayer.Services;
using FantasticMusicPlayer.SongProviders;
using FantasticMusicPlayer.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace FantasticMusicPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public IHost AppHost => _host;

        public static App CurrentApp => App.Current as App;

        public T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T ?? throw new Exception();

        public App()
        {
        }

        public static string APP_LOCATION = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static void Invoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BassUtils.CheckLibrary();

            if (!File.Exists("收藏.playlist")) { File.Create("收藏.playlist").Dispose(); }

            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureLogging(l =>
                    {
                        l.AddConsole();
                    })
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(APP_LOCATION);
                        c.AddJsonFile("appsettings.json");
                        c.AddJsonFile("appsettings.dev.json", optional: true);
                    })
                    .ConfigureServices(ConfigureServices)
                    .Build();

            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // TODO: Register services, viewmodels and pages here
            services.AddTransient<IShellWindow,MainWindow>();

            services.AddSingleton<IBassPlayer, BassPlayerImpl>();
            services.AddSingleton<PlayerController>();

            services.AddTransient<IPlayListProvider, CurrentDirectorySongProvider>();

            // App Host
            services.AddHostedService<ApplicationHostService>();

        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }

}
