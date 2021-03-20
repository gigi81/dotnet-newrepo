using Grillisoft.DotnetTools.NewRepo.Creators;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal sealed class Program
    {
        const string InitCommand = "init";

        static async Task Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).RunConsoleAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                //ctrl+C support
                .UseConsoleLifetime(options =>
                {
                    options.SuppressStatusMessages = true;
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders()
                           .AddLog4Net();
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
                            .AddSingleton<ICreator, GitAttributesCreator>()
                            .AddSingleton<ICreator, DotnetCreator>()
                            .AddSingleton<ICreator, DirectoryBuildPropsCreator>()
                            .AddSingleton<ICreator, LicenseCreator>()
                            .AddSingleton<ICreator, ReadmeCreator>()
                            .AddSingleton<ICreator, AzurePipelinesCreator>()
                            .AddSingleton<ICreator, IssueTrackerCreator>()
                            .AddSingleton<ICreator, BenchmarkDotNetCreator>()
                            //this MUST be the LAST one as it initialize the git repo and does initial commit
                            .AddSingleton<ICreator, GitCreator>();
                });
        }
    }
}
