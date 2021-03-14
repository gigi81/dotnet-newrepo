using Grillisoft.DotnetTools.NewRepo.Creators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class NewRepoService : BackgroundService
    {
        private readonly NewRepoSettings _options;
        private readonly IEnumerable<ICreator> _creators;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public NewRepoService(
            NewRepoSettings options,
            IEnumerable<ICreator> creators,
            ILogger<NewRepoService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _options = options;
            _creators = creators;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                _logger.LogInformation("Creating dotnet repo in {0}", _options.Root.FullName);

                var batch = new List<Task>();

                foreach (var creator in _creators)
                {
                    if (creator.IsParallel)
                    {
                        batch.Add(creator.Create(stoppingToken));
                        continue;
                    }

                    await Task.WhenAll(batch);
                    await creator.Create(stoppingToken);
                    batch.Clear();
                }

                await Task.WhenAll(batch);

                _logger.LogInformation("Repository {0} created in {1}", _options.Root.FullName, watch.Elapsed);
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
