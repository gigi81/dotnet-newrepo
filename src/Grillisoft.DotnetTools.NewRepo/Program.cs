using Grillisoft.DotnetTools.NewRepo.Creators;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class Program
    {
        const string InitCommand = "init";

        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime() //ctrl+C support
                .ConfigureLogging(logging =>
                {
                    logging.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    if (args.Contains(InitCommand))
                        services.AddHostedService<InitService>();
                    else
                        services.AddHostedService<NewRepoService>();

                    args = args.Except(new[] { InitCommand }).ToArray();

                    services.AddHttpClient()
                            .AddSingleton(new NewRepoSettings(args))
                            //this MUST be the FIRST one as it creates the main directories
                            .AddSingleton<ICreator, RepositoryCreator>()
                            .AddSingleton<ICreator, GitIgnoreCreator>()
                            .AddSingleton<ICreator, DotnetCreator>()
                            .AddSingleton<ICreator, DirectoryBuildPropsCreator>()
                            .AddSingleton<ICreator, LicenseCreator>()
                            .AddSingleton<ICreator, ReadmeCreator>()
                            .AddSingleton<ICreator, AzurePipelinesCreator>()
                            //this MUST be the LAST one as it creates the repo and does initial commit
                            .AddSingleton<ICreator, GitCreator>();
                });
        }
    }
}
