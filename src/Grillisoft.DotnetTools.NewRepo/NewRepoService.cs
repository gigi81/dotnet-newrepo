using Grillisoft.DotnetTools.NewRepo.Creators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class NewRepoService : IHostedService
    {
        private readonly NewRepoOptions _options;
        private readonly IEnumerable<ICreator> _creators;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public NewRepoService(
            NewRepoOptions options,
            IEnumerable<ICreator> creators,
            ILogger<NewRepoService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _options = options;
            _creators = creators;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken token)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Creating dotnet repo in {0}", _options.Root.FullName);

                        foreach (var creator in _creators)
                            await creator.Create(token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
