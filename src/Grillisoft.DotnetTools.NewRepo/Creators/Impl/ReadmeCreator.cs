using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class ReadmeCreator : CreatorBase
    {
        public string Name = "README.md";

        public ReadmeCreator(
            NewRepoSettings options,
            ILogger<ReadmeCreator> logger)
            : base(options, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            await this.CreateTextFile(this.Root.File(Name), GetContent(), _logger);
        }

        private string GetContent()
        {
            throw new NotImplementedException();
        }
    }
}
