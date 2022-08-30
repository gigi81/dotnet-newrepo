using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class GitCreator : CreatorBase
    {
        public GitCreator(
            INewRepoSettings settings,
            ILogger<GitCreator> logger)
            : base(settings, logger)
        {
        }

        public override bool IsParallel => false;

        public async override Task Create(CancellationToken cancellationToken)
        {
            await Run("git", "init", cancellationToken);
            await Run("git", "add -A", cancellationToken);
            await Run("git", "commit -m \"Initial commit\"", cancellationToken);

            if (!string.IsNullOrWhiteSpace(_settings.GithubUrl))
            {
                await Run("git", $"remote add origin {_settings.GithubUrl}", cancellationToken);
                return;
            }

            if (!string.IsNullOrWhiteSpace(_settings.AzureDevOpsGitRemoteUrl))
            {
                await Run("git", $"remote add origin {_settings.AzureDevOpsGitRemoteUrl}", cancellationToken);
                return;
            }

            _logger.LogInformation($"Cannot set git remote. Not github or azure devops settings specified");
            return;
        }
    }
}
