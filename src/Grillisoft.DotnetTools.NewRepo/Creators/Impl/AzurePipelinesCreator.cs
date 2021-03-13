using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class AzurePipelinesCreator : CreatorBase
    {
        public const string Name = "azure-pipelines.yml";

        public AzurePipelinesCreator(
            NewRepoSettings options,
            ILogger<AzurePipelinesCreator> logger)
            : base(options, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            if (!_options.AzureDevops)
            {
                _logger.LogInformation("Skipping AzureDevops yaml creation");
                return;
            }

            await this.CreateTextFile(this.Root.File(Name), await GetTemplateContent(Name));
        }
    }
}
