using SimpleExec;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Grillisoft.DotnetTools.NewRepo.Creators
{
    public abstract class CreatorBase : ICreator
    {
        private readonly DirectoryInfo _root;
        private readonly DirectoryInfo _src;
        private readonly DirectoryInfo _tests;

        public CreatorBase(NewRepoOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _root = options.Root;
            _src = _root.SubDirectory("src");
            _tests = _root.SubDirectory("tests");
        }

        public abstract Task Create(CancellationToken cancellationToken);

        public DirectoryInfo Root => _root;

        public DirectoryInfo Src => _src;

        public DirectoryInfo Tests => _tests;

        public IEnumerable<DirectoryInfo> All
        {
            get
            {
                yield return this.Root;
                yield return this.Src;
                yield return this.Tests;
            }
        }

        protected async Task Run(string name, string args, CancellationToken cancellationToken)
        {
            await Run(name, args, this.Root, cancellationToken);
        }

        protected async Task Run(string name, string args, DirectoryInfo workingDirectory, CancellationToken cancellationToken)
        {
            await Command.RunAsync(name, args, workingDirectory.FullName, false, null, null, null, null, false, cancellationToken);
        }

        protected async Task<string> GetTemplateContent(string resourceName)
        {
            var name = typeof(Program).Assembly.GetManifestResourceNames().First(r => r.EndsWith(resourceName));

            using (var stream = typeof(Program).Assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        protected async Task CreateTextFile(FileInfo file, string content, ILogger logger)
        {
            logger.LogInformation("Creating file {0}", file.FullName);
            await file.CreateTextFile(content);
        }
    }
}
