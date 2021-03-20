using Grillisoft.DotnetTools.NewRepo.Creators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class InitService : BackgroundService
    {
        private readonly NewRepoSettings _options;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public InitService(
            NewRepoSettings options,
            ILogger<NewRepoService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _options = options;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var init = _options.Root.File("init.json");
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                using (var stream = init.OpenWrite())
                    await JsonSerializer.SerializeAsync(stream, _options, jsonOptions, stoppingToken);

                _logger.LogInformation($"Created file {init.FullName}");
                _logger.LogInformation($"Customize your settings in the file then, on the same folder, run: dotnet newrepo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating dotnet repo:" + ex.Message);
            }
            finally
            {
                // Stop the application once the work is done
                _appLifetime.StopApplication();
            }
        }
    }
}
