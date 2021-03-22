using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Abstractions
{
    public interface INewRepoSettings
    {
        DirectoryInfo Root { get; }

        FileInfo InitFile { get; }

        bool Appveyor => GetBool(ConfigurationKeysManager.Appveyor);
        string Authors => GetString(ConfigurationKeysManager.Authors);
        bool AzureDevops => GetBool(ConfigurationKeysManager.AzureDevops);
        bool Benchmark => GetBool(ConfigurationKeysManager.Benchmark);
        string CopyrightYear => GetString(ConfigurationKeysManager.CopyrightYears);
        string Github => GetString(ConfigurationKeysManager.Github);
        string GithubUrl => string.IsNullOrWhiteSpace(this.Github) ? null : $"https://github.com/{Github}.git";
        string[] GitIgnoreTags => Get<string[]>(ConfigurationKeysManager.IgnoreTags);
        string License => GetString(ConfigurationKeysManager.License);
        string Name => GetString(ConfigurationKeysManager.Name);
        string Product => GetString(ConfigurationKeysManager.Product);
        string TestFramework => GetString(ConfigurationKeysManager.TestFramework);
        string TwitterUsername => GetString(ConfigurationKeysManager.Twitter);

        string GetString(ConfigurationKey key);

        int GetInt32(ConfigurationKey key);

        bool GetBool(ConfigurationKey key);

        T Get<T>(ConfigurationKey key);

        bool TryGet<T>(ConfigurationKey key, out T value);

        Task Load(ILogger logger, CancellationToken cancellationToken);

        Task Init(ILogger logger, CancellationToken cancellationToken);
    }
}
