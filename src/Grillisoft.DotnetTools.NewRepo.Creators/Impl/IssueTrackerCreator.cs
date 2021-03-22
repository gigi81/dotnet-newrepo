using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class IssueTrackerCreator : CreatorBase
    {
        public const string Name = ".issuetracker";

        public IssueTrackerCreator(
            INewRepoSettings settings,
            ILogger<IssueTrackerCreator> logger)
            : base(settings, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.GithubUrl))
            {
                _logger.LogInformation($"Skipping {Name} creation. Not github settings specified");
                return;
            }

            var content = (await GetTemplateContent(Name))
                .Replace("github_url", _settings.GithubUrl);

            await this.CreateTextFile(this.Root.File(Name), content);
        }
    }
}
