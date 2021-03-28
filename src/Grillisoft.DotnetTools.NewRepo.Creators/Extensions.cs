using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    public static class Extensions
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

        public static Task<string[]> ReadAllLinesAsync(this FileInfo file, CancellationToken cancellationToken = default)
        {
            return ReadAllLinesAsync(file, Encoding.UTF8, cancellationToken);
        }

        public static async Task<string[]> ReadAllLinesAsync(this FileInfo file, Encoding encoding, CancellationToken cancellationToken = default)
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

        public static Task WriteAllLinesAsync(this FileInfo file, IEnumerable<string> lines, CancellationToken cancellationToken = default)
        {
            return WriteAllLinesAsync(file, lines, UTF8WithoutBom, cancellationToken);
        }

        public static Task WriteAllLinesAsync(this FileInfo file, IEnumerable<string> lines, Encoding encoding, CancellationToken cancellationToken = default)
        {
            return WriteText(file, lines.JoinLines(), encoding, cancellationToken);
        }

        public static Task WriteText(this FileInfo file, StringBuilder builder, CancellationToken cancellationToken = default)
        {
            return WriteText(file, builder, UTF8WithoutBom, cancellationToken);
        }

        public static async Task WriteText(this FileInfo file, StringBuilder builder, Encoding encoding, CancellationToken cancellationToken = default)
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

        public static bool IsSelfEnumerable(this Type type)
        {
            bool isDirectly = type == typeof(IEnumerable<>);
            return isDirectly;
        }

        public static bool IsTypeDefinitionEnumerable(this Type type)
        {
            bool isViaInterfaces = type.IsGenericType &&
                                   type.GetGenericTypeDefinition().IsSelfEnumerable();
            return isViaInterfaces;
        }

        /// <summary>Check whether the specified type is enumerable.</summary>
        /// <param name="type">The type.</param>
        /// <param name="underlyingType">IEnumerable{int} would be int</param>
        /// <param name="excludeString">
        ///  [OPTIONAL] if set to <c>true</c> [exclude string]. Strings are enumerable as char[]
        ///  this is likely not something you want. Default is true (string will return false)
        /// </param>
        /// <returns><c>true</c> supplied type is enumerable otherwise <c>false</c></returns>
        public static bool IsEnumerable(this Type type, out Type underlyingType,
                                        bool excludeString = true)
        {
            underlyingType = null;

            if (type.IsEnum || type.IsPrimitive || type.IsValueType) return false;

            if (excludeString && type == typeof(string)) return false;

            if (type.IsGenericType)
            {
                if (type.IsTypeDefinitionEnumerable() ||
                    type.GetInterfaces()
                        .Any(t => t.IsSelfEnumerable() || t.IsTypeDefinitionEnumerable()))
                {
                    underlyingType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            //direct implementations of IEnumerable<T>, inheritance from List<T> etc
            var enumerableOrNull = type.GetInterfaces()
                                       .FirstOrDefault(t => t.IsTypeDefinitionEnumerable());
            if (enumerableOrNull == null) return false;

            underlyingType = enumerableOrNull.GetGenericArguments()[0];
            return true;
        }
    }
}
