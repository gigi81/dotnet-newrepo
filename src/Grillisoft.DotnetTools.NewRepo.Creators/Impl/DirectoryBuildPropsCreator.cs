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
        private const string DirectoryBuildPropsName = "Directory.Build.props";
        private const string DirectoryPackagePropsName = "Directory.Packages.props";

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

            //packages props
            var content = await GetTemplateContent(DirectoryPackagePropsName);
            await this.CreateTextFile(this.Root.File(DirectoryPackagePropsName), content);

            //build props
            foreach(var entry in dirs)
            {
                content = await GetTemplateContent(entry.Value + DirectoryBuildPropsName);
                await this.CreateTextFile(entry.Key.File(DirectoryBuildPropsName), content);
            }
        }
    }
}
