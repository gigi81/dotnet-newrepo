namespace Grillisoft.DotnetTools.NewRepo.Abstractions
{
    public interface INewRepoSettings
    {
        //DirectoryInfo Root { get; }
        //FileInfo InitFile { get; }

        bool Appveyor => GetBool(ConfigurationKeys.Appveyor);
        string Authors => GetString(ConfigurationKeys.Authors);
        bool AzureDevops => GetBool(ConfigurationKeys.AzureDevops);
        bool Benchmark => GetBool(ConfigurationKeys.Benchmark);
        string CopyrightYear => GetString(ConfigurationKeys.CopyrightYears);
        string Github => GetString(ConfigurationKeys.Github);
        string GithubUrl => $"https://github.com/{Github}.git";
        string[] GitIgnoreTags => Get<string[]>(ConfigurationKeys.IgnoreTags);
        string License => GetString(ConfigurationKeys.License);
        string Name => GetString(ConfigurationKeys.Name);
        string Product => GetString(ConfigurationKeys.Product);
        string TestFramework => GetString(ConfigurationKeys.TestFramework);
        string TwitterUsername => GetString(ConfigurationKeys.Twitter);

        string GetString(ConfigurationKey key);

        int GetInt32(ConfigurationKey key);

        bool GetBool(ConfigurationKey key);

        T Get<T>(ConfigurationKey key);

        bool TryGet<T>(ConfigurationKey key, out T value);
    }
}
