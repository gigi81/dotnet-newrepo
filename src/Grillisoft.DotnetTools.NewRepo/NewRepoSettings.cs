using System;
using System.IO;

namespace Grillisoft.DotnetTools.NewRepo
{
    public sealed class NewRepoSettings
    {
        private DirectoryInfo _root;
        private string _name;
        private string _copyrightOwner;
        private int _copyrightYear;

        public NewRepoSettings(string[] args)
        {
            _root = new DirectoryInfo(args.Length > 0 ? args[0] : ".");
        }

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

        public int CopyrightYear
        {
            get => _copyrightYear > 0 ? _copyrightYear : DateTime.UtcNow.Year;
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

        private static string GetOrDefault(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}
