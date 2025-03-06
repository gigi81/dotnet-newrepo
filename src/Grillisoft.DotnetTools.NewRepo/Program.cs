using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Grillisoft.DotnetTools.NewRepo.Configuration.Yaml;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.Net;
using System.Net.Http;

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
                if(Environment.ExitCode == ExitCode.Ok)
                    Environment.ExitCode = ExitCode.GenericError;

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
                .UseSerilog((hostingContext, services, loggerConfiguration) =>
                {
                    loggerConfiguration.Enrich.FromLogContext()
                                       .WriteTo.Console();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    if (args.Contains(InitCommand))
                        services.AddHostedService<InitService>();
                    else
                        services.AddHostedService<NewRepoService>();

                    args = args.Except(new[] { InitCommand }).ToArray();

                    var handler = new HttpClientHandler();
                    handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    
                    services.AddHttpClient()
                            .ConfigureHttpClientDefaults(builder => builder.ConfigurePrimaryHttpMessageHandler(() => handler))
                            .AddSingleton<IFileSystem, FileSystem>()
                            .AddSingleton<INewRepoSettings>(provider => new YamlNewRepoSettings(args, provider.GetRequiredService<IFileSystem>()))
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
