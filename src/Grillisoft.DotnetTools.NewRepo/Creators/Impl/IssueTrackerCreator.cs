using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class IssueTrackerCreator : CreatorBase
    {
        public const string Name = ".issuetracker";

        public IssueTrackerCreator(
            NewRepoSettings options,
            ILogger<IssueTrackerCreator> logger)
            : base(options, logger)
        {
        }

        public override async Task Create(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.GithubUsername) ||
                string.IsNullOrWhiteSpace(_options.GithubRepoName))
            {
                _logger.LogInformation($"Skipping {Name} creation. Not github settings specified");
                return;
            }

            var content = (await GetTemplateContent(Name))
                .Replace("username", _options.GithubUsername)
                .Replace("repository", _options.GithubRepoName);

            await this.CreateTextFile(this.Root.File(Name), content);
        }
    }
}
