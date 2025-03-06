using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class DirectoryBuildPropsCreator : CreatorBase
    {
        private const string Name = "Directory.Build.props";
        private const string CentralPackageManagementName = "Directory.Packages.props";

        public DirectoryBuildPropsCreator(
            INewRepoSettings settings,
            ILogger<DirectoryBuildPropsCreator> logger)
            : base(settings, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken cancellationToken)
        {
            var dirs = new Dictionary<IDirectoryInfo, string>
            {
                { this.Root,  "Root"  },
                { this.Src,   "Src"   },
                { this.Tests, "Tests" }
            };

            foreach(var entry in dirs)
            {
                var content = await GetTemplateContent(entry.Value + Name);
                await this.CreateTextFile(entry.Key.File(Name), content);
            }

            if (_settings.CentralPackageManagement)
            {
                var content = await GetTemplateContent(CentralPackageManagementName);
                await this.CreateTextFile(this.Root.File(CentralPackageManagementName), content);
            }
        }
    }
}
