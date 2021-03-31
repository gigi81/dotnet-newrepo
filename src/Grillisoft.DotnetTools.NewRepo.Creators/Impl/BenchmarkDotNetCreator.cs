using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class BenchmarkDotNetCreator : CreatorBase
    {
        public BenchmarkDotNetCreator(
            INewRepoSettings settings,
            ILogger<BenchmarkDotNetCreator> logger)
            : base(settings, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            if(!_settings.Benchmark)
            {
                _logger.LogInformation("Skipping benchmark project creation");
                return;
            }

            var benchmarkDir = this.Root.SubDirectory("benchmark");
            if(!benchmarkDir.Exists)
                benchmarkDir.Create();

            var projectDir = benchmarkDir.SubDirectory(_settings.Name + ".Benchmark");
            if (!projectDir.Exists)
                projectDir.Create();

            try
            {
                await Run("dotnet", "new --install BenchmarkDotNet.Templates::0.12.1", projectDir, cancellationToken);
            }
            catch(Exception)
            {
                _logger.LogWarning("Failed to install BenchmarkDotNet.Templates");
            }

            await Run("dotnet", "new benchmark", projectDir, cancellationToken);

            var project = projectDir.GetFiles("*.csproj").First();
            await Run("dotnet", $"sln add \"{project.FullName}\"", this.Root, cancellationToken);
        }
    }
}
