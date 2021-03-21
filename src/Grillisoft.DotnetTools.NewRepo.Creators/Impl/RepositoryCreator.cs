using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class RepositoryCreator : CreatorBase
    {
        public RepositoryCreator(
            NewRepoSettings settings,
            ILogger<RepositoryCreator> logger)
            : base(settings, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken token)
        {
            if (!this.Root.Exists)
                this.Root.Create();

            await _settings.LoadSettings(_logger, token);

            if (this.Root.GetFiles().Where(f => !f.Name.Equals(NewRepoSettings.InitFilename)).Count() > 0 ||
                this.Root.GetDirectories().Length > 0)
                throw new RepositoryDirectoryNotEmpty(this.Root.FullName);

            this.Src.Create();
            this.Tests.Create();
        }
    }
}
