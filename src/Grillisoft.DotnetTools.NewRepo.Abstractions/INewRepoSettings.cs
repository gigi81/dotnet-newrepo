using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Abstractions
{
    public interface INewRepoSettings
    {
        IDirectoryInfo Root { get; }

        IFileInfo InitFile { get; }

        bool Appveyor => GetBool(ConfigurationKeysManager.Appveyor);
        string Authors => GetString(ConfigurationKeysManager.Authors);
        bool Benchmark => GetBool(ConfigurationKeysManager.Benchmark);
        string CopyrightYear => GetString(ConfigurationKeysManager.CopyrightYears);
        string Github => GetString(ConfigurationKeysManager.Github);
        string GithubUrl => string.IsNullOrWhiteSpace(this.Github) ? null : $"https://github.com/{Github}.git";
        string AzureDevOpsGitRemoteUrl => GetString(ConfigurationKeysManager.AzureDevOpsGitRemoteUrl);
        bool AzureDevOpsBuild => GetBool(ConfigurationKeysManager.AzureDevOpsBuild);
        string[] GitIgnoreTags => Get<string[]>(ConfigurationKeysManager.IgnoreTags);
        string License => GetString(ConfigurationKeysManager.License);
        string Name => GetString(ConfigurationKeysManager.Name);
        string Product => GetString(ConfigurationKeysManager.Product);
        string TestFramework => GetString(ConfigurationKeysManager.TestFramework);
        string TwitterUsername => GetString(ConfigurationKeysManager.Twitter);
        bool EmptyReadme => GetBool(ConfigurationKeysManager.EmptyReadme);
        string GetString(ConfigurationKey key);

        int GetInt32(ConfigurationKey key);

        bool GetBool(ConfigurationKey key);

        T Get<T>(ConfigurationKey key);

        bool TryGet<T>(ConfigurationKey key, out T value);

        Task Load(ILogger logger, CancellationToken cancellationToken);

        Task Init(ILogger logger, CancellationToken cancellationToken);
    }
}
