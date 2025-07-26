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

        public override async Task Create(CancellationToken cancellationToken)
        {
            await Run("git", "init", cancellationToken);
            await Run("git", "add -A", cancellationToken);
            await Run("git", "commit -m \"Initial commit\"", cancellationToken);

            if (string.IsNullOrWhiteSpace(_settings.GitRemoteUrl))
            {
                _logger.LogWarning("Could not set git remote. No github or azure devops settings specified");
                return;
            }
            
            _logger.LogInformation("Setting git remote to {GitRemoteUrl}", _settings.GitRemoteUrl);
            await Run("git", $"remote add origin {_settings.GitRemoteUrl}", cancellationToken);
        }
    }
}
