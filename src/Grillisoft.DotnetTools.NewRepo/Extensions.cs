using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    public static class Extensions
    {
        public static readonly Encoding UTF8WithoutBom = new UTF8Encoding(false);

        public static DirectoryInfo SubDirectory(this DirectoryInfo root, string name)
        {
            return new DirectoryInfo(Path.Combine(root.FullName, name));
        }

        public static FileInfo File(this DirectoryInfo root, string name)
        {
            return new FileInfo(Path.Combine(root.FullName, name));
        }

        public static async Task CreateTextFile(this FileInfo file, string content)
        {
            await CreateTextFile(file, content, UTF8WithoutBom);
        }

        public static async Task CreateTextFile(this FileInfo file, string content, Encoding encoding)
        {
            using(var stream = file.Open(FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream, encoding))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
