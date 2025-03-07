﻿using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grillisoft.DotnetTools.NewRepo.Creators.Exceptions;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class RepositoryCreator : CreatorBase
    {
        public RepositoryCreator(
            INewRepoSettings settings,
            ILogger<RepositoryCreator> logger)
            : base(settings, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken token)
        {
            if (!this.Root.Exists)
                this.Root.Create();

            var initFileName = _settings.InitFile.Name;

            await _settings.Load(_logger, token);

            if (this.Root.GetFiles().Where(f => !f.Name.Equals(initFileName)).Count() > 0 ||
                this.Root.GetDirectories().Length > 0)
                throw new RepositoryDirectoryNotEmpty(this.Root.FullName, initFileName);

            this.Src.Create();
            this.Tests.Create();
        }
    }
}
