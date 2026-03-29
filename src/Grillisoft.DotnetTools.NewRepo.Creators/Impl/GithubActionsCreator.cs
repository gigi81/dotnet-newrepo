using System.IO.Abstractions;
using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class GithubActionsCreator : CreatorBase
    {
        public const string TemplateName = "github-actions.yml";
        public const string WorkflowFileName = "ci.yml";

        public GithubActionsCreator(
            INewRepoSettings settings,
            ILogger<GithubActionsCreator> logger)
            : base(settings, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            if (!_settings.GithubActionsBuild)
            {
                _logger.LogInformation("Skipping GitHub Actions yaml creation");
                return;
            }

            var githubDir = this.Root.SubDirectory(".github");
            if (!githubDir.Exists)
                githubDir.Create();

            var workflowsDir = githubDir.SubDirectory("workflows");
            if (!workflowsDir.Exists)
                workflowsDir.Create();

            await this.CreateTextFile(workflowsDir.File(WorkflowFileName), await GetTemplateContent(TemplateName));
        }
    }
}
