using System;
using System.Collections.Generic;
using System.Linq;

namespace Grillisoft.DotnetTools.NewRepo.Abstractions
{
    public class ConfigurationKeysManager
    {
        public static readonly ConfigurationKey Name = StringKey("name", "Project name (ex. YourOrganization.Project)");
        public static readonly ConfigurationKey Authors = StringKey("authors", "Authors of the project, this could be your name or your company Name");
        public static readonly ConfigurationKey Product = StringKey("product", "Product description");
        public static readonly ConfigurationKey Github = StringKey("github", "The github repository name where this project will be hosted (ex. yourUsername/repo)");
        public static readonly ConfigurationKey CopyrightYears = StringKey("copyrightYears", "Copyright years (ex. 2021 or 2010-2021)", DateTime.Now.Year.ToString());
        public static readonly ConfigurationKey License = StringKey("license", "The project license's SPDX identifier. Only OSI and FSF approved licenses supported, see https://spdx.org/licenses/", "MIT");
        public static readonly ConfigurationKey TestFramework = StringKey("testFramework", "Your test framework of choice (ex xunit or nunit)", "xunit");
        public static readonly ConfigurationKey IgnoreTags = new ConfigurationKey("ignoreTags", typeof(string[]), "Tags used to generate the .gitignore file, see https://www.toptal.com/developers/gitignore", new[] { "csharp", "visualstudio", "visualstudiocode" });
        public static readonly ConfigurationKey Benchmark = BoolKey("benchmark", "Set to true if you want to add a BenchmarkDotnet project added to the solution", false);
        public static readonly ConfigurationKey AzureDevops = BoolKey("azureDevops", "Set to true if you plan to build the project in azure devops", true);
        public static readonly ConfigurationKey Appveyor = BoolKey("appveyor", "Set to true if you plan to build the project in appveyor", false);
        public static readonly ConfigurationKey Twitter = StringKey("twitter", "Your Twitter account handle (ex. @john)");

        public static readonly Lazy<IDictionary<string, ConfigurationKey>> _keys = new Lazy<IDictionary<string, ConfigurationKey>>(GetKeys);

        public static IDictionary<string, ConfigurationKey> Keys => _keys.Value;

        private static Dictionary<string, ConfigurationKey> GetKeys()
        {
            var keys = typeof(ConfigurationKeysManager).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                        .Where(f => f.FieldType == typeof(ConfigurationKey))
                        .Select(f => f.GetValue(null) as ConfigurationKey);

            var ret = new Dictionary<string, ConfigurationKey>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in keys)
                ret.Add(key.Key, key);

            return ret;
        }

        private static ConfigurationKey StringKey(string name, string help, object defaultValue = null)
        {
            return new ConfigurationKey
            {
                Key = name,
                Type = typeof(string),
                Help = help,
                DefaultValue = defaultValue ?? String.Empty
            };
        }

        private static ConfigurationKey BoolKey(string name, string help, object defaultValue = null)
        {
            return new ConfigurationKey
            {
                Key = name,
                Type = typeof(bool),
                Help = help,
                DefaultValue = defaultValue
            };
        }
    }
}
