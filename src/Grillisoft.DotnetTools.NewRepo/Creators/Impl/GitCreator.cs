using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class GitCreator : CreatorBase
    {
        public GitCreator(
            NewRepoSettings settings,
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

            if (string.IsNullOrWhiteSpace(_settings.GithubUrl))
            {
                _logger.LogInformation($"Cannot set git remote. Not github settings specified");
                return;
            }

            await Run("git", $"remote add origin {_settings.GithubUrl}", cancellationToken);
        }
    }
}
