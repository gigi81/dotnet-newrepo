using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class ReadmeCreator : CreatorBase
    {
        public string Name = "README.md";

        private readonly IHttpClientFactory _httpClientFactory;

        public ReadmeCreator(
            NewRepoSettings settings,
            ILogger<ReadmeCreator> logger,
            IHttpClientFactory httpClientFactory)
            : base(settings, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string Url => $"https://raw.githubusercontent.com/othneildrew/Best-README-Template/master/BLANK_README.md";

        public override async Task Create(CancellationToken cancellationToken)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                _logger.LogInformation("Downloading {0} from {1}", Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                responseBody = responseBody.Replace("github_username", _settings.GithubUsername);
                responseBody = responseBody.Replace("repo_name", _settings.GithubRepoName);
                responseBody = responseBody.Replace("twitter_handle", _settings.TwitterUsername);
                responseBody = responseBody.Replace("project_title", _settings.Name);
                responseBody = responseBody.Replace("project_description", _settings.Product);

                var index = responseBody.IndexOf("-->");
                if (index > 0)
                    responseBody = responseBody.Substring(index + 3);

                await this.CreateTextFile(this.Root.File(Name), responseBody);
            }
        }
    }
}
