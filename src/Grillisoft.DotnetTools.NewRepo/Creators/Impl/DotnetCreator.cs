using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class DotnetCreator : CreatorBase
    {
        public DotnetCreator(
            NewRepoSettings options,
            ILogger<DotnetCreator> logger)
            : base(options, logger)
        {
        }

        public async override Task Create(CancellationToken cancellationToken)
        {
            await Run("dotnet", "new sln", cancellationToken);

            var src = this.Src.CreateSubdirectory(_options.Name);
            await Run("dotnet", "new classlib", src, cancellationToken);

            var tests = this.Tests.CreateSubdirectory(_options.Name + ".Tests");
            await Run("dotnet", "new " + _options.TestFramework, tests, cancellationToken);

            foreach (var prj in this.Root.GetFiles("*.csproj", System.IO.SearchOption.AllDirectories))
                await Run("dotnet", "sln add \"" + prj.FullName + "\"", cancellationToken);
        }
    }
}
