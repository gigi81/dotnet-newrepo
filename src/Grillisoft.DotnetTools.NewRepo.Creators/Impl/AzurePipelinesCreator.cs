using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class AzurePipelinesCreator : CreatorBase
    {
        public const string Name = "azure-pipelines.yml";
        public AzurePipelinesCreator(
            INewRepoSettings settings,
            ILogger<AzurePipelinesCreator> logger)
            : base(settings, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            if (!_settings.AzureDevOpsBuild)
            {
                _logger.LogInformation("Skipping AzureDevops yaml creation");
                return;
            }

            await this.CreateTextFile(this.Root.File(Name), await GetTemplateContent(Name));
        }
    }
}
