using System.IO;

namespace Grillisoft.DotnetTools.NewRepo
{
    public sealed class NewRepoOptions
    {
        private DirectoryInfo _root;

        public NewRepoOptions(string[] args)
        {
            _root = new DirectoryInfo(args.Length > 0 ? args[0] : ".");
        }

        public DirectoryInfo Root => _root;

        public string Name => _root.Name;

        public string Authors { get; set; }

        public string Product { get; set; }

        public string TestFramework { get; set; } = "xunit";

        public string[] GitIgnoreTags { get; set; } = new[] { "csharp", "visualstudio", "visualstudiocode" };
    }
}
