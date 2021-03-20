using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Grillisoft.DotnetTools.NewRepo
{
    public sealed class NewRepoSettings
    {
        private readonly DirectoryInfo _root;
        private string _name;
        private string _copyrightOwner;
        private string _copyrightYear;
        private string _githubRepoName;

        public NewRepoSettings()
        {
            _root = new DirectoryInfo(".");
        }

        public NewRepoSettings(string[] args)
        {
            _root = new DirectoryInfo(args.Length > 0 ? args[0] : ".");
        }

        [JsonIgnore]
        public DirectoryInfo Root => _root;

        public string Name
        {
            get => _name ?? _root.Name;
            set => _name = value;
        }

        public string Authors { get; set; }

        public string Product { get; set; }

        public string CopyrightHolders
        {
            get => GetOrDefault(_copyrightOwner, this.Authors);
            set => _copyrightOwner = value;
        }

        public string CopyrightYear
        {
            get => GetOrDefault(_copyrightYear, DateTime.UtcNow.Year.ToString());
            set => _copyrightYear = value;
        }

        /// <summary>
        /// The project license's <see cref="https://spdx.org/licenses/">SPDX identifier</see>.
        /// Only OSI and FSF approved licenses supported
        /// </summary>
        public string License { get; set; } = "MIT";

        public string TestFramework { get; set; } = "xunit";

        public string[] GitIgnoreTags { get; set; } = new[] { "csharp", "visualstudio", "visualstudiocode" };

        public bool Benchmark { get; set; } = false;

        public bool AzureDevops { get; set; } = true;

        public bool Appveyor { get; set; } = true;

        public string GithubUsername { get; set; }

        public string GithubRepoName
        {
            get => GetOrDefault(_githubRepoName, this.Name);
            set => _githubRepoName = value; }

        public string TwitterUsername { get; set; }

        private static string GetOrDefault(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public void Load(NewRepoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var fields = typeof(NewRepoSettings)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => !f.Name.Equals(nameof(_root)));

            foreach (var field in fields)
                field.SetValue(this, field.GetValue(settings));
        }
    }
}
