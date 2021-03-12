using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class RepositoryCreator : CreatorBase
    {
        public RepositoryCreator(NewRepoOptions options)
            : base(options)
        {
        }

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
