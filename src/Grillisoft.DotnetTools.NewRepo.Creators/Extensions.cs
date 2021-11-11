using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    internal static class Extensions
    {
        public static readonly Encoding UTF8WithoutBom = new UTF8Encoding(false);

        /// <summary>
        /// This is the same default buffer size as
        /// <see cref="StreamReader"/> and <see cref="FileStream"/>.
        /// </summary>
        private const int DefaultBufferSize = 4096;

        /// <summary>
        /// Indicates that
        /// 1. The file is to be used for asynchronous reading.
        /// 2. The file is to be accessed sequentially from beginning to end.
        /// </summary>
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public static async Task CreateTextFile(this IFileInfo file, string content)
        {
            await CreateTextFile(file, content, UTF8WithoutBom);
        }

        public static async Task CreateTextFile(this IFileInfo file, string content, Encoding encoding)
        {
            using(var stream = file.Open(FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream, encoding))
            {
                await writer.WriteAsync(content);
            }
        }

        public static Task<string[]> ReadAllLinesAsync(this IFileInfo file, CancellationToken cancellationToken = default)
        {
            return ReadAllLinesAsync(file, Encoding.UTF8, cancellationToken);
        }

        public static async Task<string[]> ReadAllLinesAsync(this IFileInfo file, Encoding encoding, CancellationToken cancellationToken = default)
        {
            var lines = new List<string>();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }

        public static Task WriteAllLinesAsync(this IFileInfo file, IEnumerable<string> lines, CancellationToken cancellationToken = default)
        {
            return WriteAllLinesAsync(file, lines, UTF8WithoutBom, cancellationToken);
        }

        public static Task WriteAllLinesAsync(this IFileInfo file, IEnumerable<string> lines, Encoding encoding, CancellationToken cancellationToken = default)
        {
            return WriteText(file, lines.JoinLines(), encoding, cancellationToken);
        }

        public static Task WriteText(this IFileInfo file, StringBuilder builder, CancellationToken cancellationToken = default)
        {
            return WriteText(file, builder, UTF8WithoutBom, cancellationToken);
        }

        public static async Task WriteText(this IFileInfo file, StringBuilder builder, Encoding encoding, CancellationToken cancellationToken = default)
        {
            using (var stream = new FileStream(file.FullName, file.Exists ? FileMode.Truncate : FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, DefaultBufferSize, DefaultOptions))
            using (var writer = new StreamWriter(stream, encoding))
                await writer.WriteAsync(builder, cancellationToken);
        }

        public static StringBuilder JoinLines(this IEnumerable<string> lines)
        {
            var builder = new StringBuilder();
            foreach (var line in lines)
                builder.AppendLine(line);

            return builder;
        }
    }
}
