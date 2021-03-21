using SimpleExec;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Grillisoft.DotnetTools.NewRepo.Abstractions;

namespace Grillisoft.DotnetTools.NewRepo.Creators
{
    public abstract class CreatorBase : ICreator
    {
        private readonly DirectoryInfo _root;
        private readonly DirectoryInfo _src;
        private readonly DirectoryInfo _tests;
        protected readonly NewRepoSettings _settings;
        protected readonly ILogger _logger;

        public CreatorBase(NewRepoSettings settings, ILogger logger)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;
            _logger = logger;
            _root = _settings.Root;
            _src = _root.SubDirectory("src");
            _tests = _root.SubDirectory("tests");
        }

        public virtual bool IsParallel => true;

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
            _logger.LogInformation("Running command {0} {1}", name, args);
            await Command.RunAsync(name, args, workingDirectory.FullName, false, null, null, null, null, false, cancellationToken);
        }

        protected async Task<string> GetTemplateContent(string resourceName)
        {
            var name = typeof(CreatorBase).Assembly.GetManifestResourceNames().First(r => r.EndsWith(resourceName));

            using (var stream = typeof(CreatorBase).Assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        protected async Task CreateTextFile(FileInfo file, string content)
        {
            _logger.LogInformation("Creating file {0}", file.FullName);
            await file.CreateTextFile(content);
        }
    }
}
