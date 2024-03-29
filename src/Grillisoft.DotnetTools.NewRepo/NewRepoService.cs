﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grillisoft.DotnetTools.NewRepo.Abstractions;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class NewRepoService : BackgroundService
    {
        private readonly INewRepoSettings _settings;
        private readonly IEnumerable<ICreator> _creators;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public NewRepoService(
            INewRepoSettings settings,
            IEnumerable<ICreator> creators,
            ILogger<NewRepoService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _settings = settings;
            _creators = creators;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                _logger.LogInformation("Creating dotnet repo in {0}", _settings.Root.FullName);
                await RunCreators(stoppingToken);
                _logger.LogInformation("Repository {0} created in {1}", _settings.Root.FullName, watch.Elapsed);
                Environment.ExitCode = ExitCode.Ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating dotnet repo: " + ex.Message);
                Environment.ExitCode = ExitCode.GenericError;
            }
            finally
            {
                // Stop the application once the work is done
                _appLifetime.StopApplication();
            }
        }

        private async Task RunCreators(CancellationToken stoppingToken)
        {
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
        }
    }
}
