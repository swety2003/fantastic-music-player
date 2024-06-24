using FantasticMusicPlayer.Contracts;
using FantasticMusicPlayer.Contracts.Activation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FantasticMusicPlayer.Services
{
    internal class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private bool _isInitialized;

        IConfiguration configuration;
        private IShellWindow _shellWindow;

        public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers, IConfiguration appConfigManager)
        {
            _serviceProvider = serviceProvider;
            _activationHandlers = activationHandlers;
            this.configuration = appConfigManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Initialize services that you need before app activation
            await InitializeAsync();

            await HandleActivationAsync();

            // Tasks after activation
            await StartupAsync();
            _isInitialized = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                await Task.CompletedTask;
            }
        }

        private async Task StartupAsync()
        {
            if (!_isInitialized)
            {
                await Task.CompletedTask;
            }
        }

        private async Task HandleActivationAsync()
        {
            var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle());

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync();
            }

            await Task.CompletedTask;

            if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
            {
                // Default activation that navigates to the apps default page
                _shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
                _shellWindow?.ShowWindow();
                await Task.CompletedTask;
            }
        }
    }
}
