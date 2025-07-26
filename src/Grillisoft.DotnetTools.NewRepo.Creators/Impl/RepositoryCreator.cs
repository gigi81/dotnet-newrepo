using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grillisoft.DotnetTools.NewRepo.Creators.Exceptions;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class RepositoryCreator : CreatorBase
    {
        public RepositoryCreator(
            INewRepoSettings settings,
            ILogger<RepositoryCreator> logger)
            : base(settings, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken token)
        {
            if (!this.Root.Exists)
                this.Root.Create();

            var initFileName = _settings.InitFile.Name;
            await _settings.Load(_logger, token);

            if (GetRootIsNotEmpty(initFileName))
                throw new RepositoryDirectoryNotEmpty(this.Root.FullName, initFileName);

            this.Src.Create();
            this.Tests.Create();
        }

        private bool GetRootIsNotEmpty(string initFileName)
        {
            return this.Root.GetFiles().Any(f => !f.Name.Equals(initFileName)) ||
                   this.Root.GetDirectories().Length > 0;
        }
    }
}
