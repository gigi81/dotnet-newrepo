using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class RepositoryCreator : CreatorBase
    {
        public const string InitFilename = "init.json";

        public RepositoryCreator(
            NewRepoSettings options,
            ILogger<RepositoryCreator> logger)
            : base(options, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken token)
        {
            if (!this.Root.Exists)
                this.Root.Create();

            var init = _options.Root.File(InitFilename);
            if (init.Exists)
            {
                try
                {
                    _logger.LogInformation("Loading settings from {0}", init.FullName);
                    using (var stream = init.OpenRead())
                        _options.Load(await JsonSerializer.DeserializeAsync<NewRepoSettings>(stream, null, token));
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Failed to load settings from {0}: {1}", init.FullName, ex.Message);
                    throw;
                }
            }

            if (this.Root.GetFiles().Where(f => !f.Name.Equals(InitFilename)).Count() > 0 ||
                this.Root.GetDirectories().Length > 0)
                throw new RepositoryDirectoryNotEmpty(this.Root.FullName);

            this.Src.Create();
            this.Tests.Create();
        }
    }
}
