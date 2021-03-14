using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
            var src = this.Src.CreateSubdirectory(_options.Name);
            var tests = this.Tests.CreateSubdirectory(_options.Name + ".Tests");

            await Task.WhenAll(
                Run("dotnet", "new sln", cancellationToken),
                Run("dotnet", "new classlib", src, cancellationToken),
                Run("dotnet", "new " + _options.TestFramework, tests, cancellationToken)
            );

            var projects = this.Root.GetFiles("*.csproj", System.IO.SearchOption.AllDirectories)
                               .Select(prj => "\"" + prj.FullName + "\"");

            await Run("dotnet", "sln add " + string.Join(" ", projects), cancellationToken);
        }
    }
}
