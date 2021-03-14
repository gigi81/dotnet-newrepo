using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class RepositoryCreator : CreatorBase
    {
        public RepositoryCreator(
            NewRepoSettings options,
            ILogger<RepositoryCreator> logger)
            : base(options, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken token)
        {
            await Task.Run(() => CreateDirectories());
        }

        private void CreateDirectories()
        {
            if (!this.Root.Exists)
                this.Root.Create();

            if (this.Root.GetFiles().Length > 0 || this.Root.GetDirectories().Length > 0)
                throw new RepositoryDirectoryNotEmpty(this.Root.FullName);

            this.Src.Create();
            this.Tests.Create();
        }
    }
}
