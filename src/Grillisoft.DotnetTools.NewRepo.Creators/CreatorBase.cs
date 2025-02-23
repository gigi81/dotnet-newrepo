using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Grillisoft.DotnetTools.NewRepo.Abstractions;
using System.IO;
using System.IO.Abstractions;
using CliWrap;

namespace Grillisoft.DotnetTools.NewRepo.Creators
{
    public abstract class CreatorBase : ICreator
    {
        private readonly IDirectoryInfo _root;
        private readonly IDirectoryInfo _src;
        private readonly IDirectoryInfo _tests;
        protected readonly INewRepoSettings _settings;
        protected readonly ILogger _logger;

        public CreatorBase(INewRepoSettings settings, ILogger logger)
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

        public IDirectoryInfo Root => _root;

        public IDirectoryInfo Src => _src;

        public IDirectoryInfo Tests => _tests;

        public IEnumerable<IDirectoryInfo> All
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

        protected async Task Run(string name, string args, IDirectoryInfo workingDirectory, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running command {0} {1}", name, args);
            await using var stdOut = Console.OpenStandardOutput();
            await using var stdErr = Console.OpenStandardError();

            await Cli.Wrap(name)
                .WithArguments(args)
                .WithWorkingDirectory(workingDirectory.FullName)
                .WithStandardOutputPipe(PipeTarget.ToStream(stdOut))
                .WithStandardErrorPipe(PipeTarget.ToStream(stdErr))
                .ExecuteAsync(cancellationToken);
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

        protected async Task CreateTextFile(IFileInfo file, string content)
        {
            _logger.LogInformation("Creating file {0}", file.FullName);
            await file.CreateTextFile(content);
        }
    }
}
