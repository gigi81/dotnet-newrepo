using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class GitCreator : CreatorBase
    {
        public GitCreator(
            NewRepoSettings options,
            ILogger<GitCreator> logger)
            : base(options, logger)
        {
        }

        public async override Task Create(CancellationToken cancellationToken)
        {
            await Run("git", "init", cancellationToken);
            await Run("git", "add -all", cancellationToken);
            await Run("git", "commit -m \"Initial commit\"", cancellationToken);
        }
    }
}
