using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Grillisoft.DotnetTools.NewRepo.Abstractions;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class InitService : BackgroundService
    {
        private readonly INewRepoSettings _settings;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public InitService(
            INewRepoSettings settings,
            ILogger<NewRepoService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _settings = settings;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var init = _settings.InitFile;

            try
            {
                await _settings.Init(_logger, stoppingToken);
                _logger.LogInformation($"Created file {init.FullName}");
                _logger.LogInformation($"Customize your settings in the file then, on the same folder, run: dotnet newrepo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while creating file {init.FullName}: " + ex.Message);
            }
            finally
            {
                // Stop the application once the work is done
                _appLifetime.StopApplication();
            }
        }
    }
}
