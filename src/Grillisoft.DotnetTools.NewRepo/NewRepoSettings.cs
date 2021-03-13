using System.IO;

namespace Grillisoft.DotnetTools.NewRepo
{
    public sealed class NewRepoSettings
    {
        private DirectoryInfo _root;
        private string _name;

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

        public string TestFramework { get; set; } = "xunit";

        public string[] GitIgnoreTags { get; set; } = new[] { "csharp", "visualstudio", "visualstudiocode" };

        public bool Benchmark { get; set; } = false;
    }
}
