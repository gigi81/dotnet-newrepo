using Grillisoft.DotnetTools.NewRepo.Creators;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<NewRepoService>()
                            .AddHttpClient()
                            .AddSingleton(new NewRepoSettings(args))
                            //this MUST be the FIRST one as it creates the main directories
                            .AddSingleton<ICreator, RepositoryCreator>()
                            .AddSingleton<ICreator, GitIgnoreCreator>()
                            .AddSingleton<ICreator, DotnetCreator>()
                            .AddSingleton<ICreator, DirectoryBuildPropsCreator>()
                            .AddSingleton<ICreator, LicenseCreator>()
                            .AddSingleton<ICreator, ReadmeCreator>()
                            //this MUST be the LAST one as it creates the repo and does initial commit
                            .AddSingleton<ICreator, GitCreator>();
                });
        }
    }
}
