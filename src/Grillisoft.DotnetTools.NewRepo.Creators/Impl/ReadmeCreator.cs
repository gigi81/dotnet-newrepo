using System;
using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
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
            INewRepoSettings settings,
            ILogger<ReadmeCreator> logger,
            IHttpClientFactory httpClientFactory)
            : base(settings, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string Url => $"https://raw.githubusercontent.com/othneildrew/Best-README-Template/master/BLANK_README.md";

        public override async Task Create(CancellationToken cancellationToken)
        {
            if(_settings.EmptyReadme)
            {
                await this.CreateTextFile(this.Root.File(Name), $"##{_settings.Name}");
                return;
            }

            var responseBody = await DownloadReadme(cancellationToken);
            if (string.IsNullOrWhiteSpace(responseBody))
                return;
            
            //responseBody = responseBody.Replace("github_username", _settings.GithubUsername);
            //responseBody = responseBody.Replace("repo_name", _settings.GithubRepoName);
            responseBody = responseBody.Replace("twitter_handle", _settings.TwitterUsername);
            responseBody = responseBody.Replace("project_title", _settings.Name);
            responseBody = responseBody.Replace("project_description", _settings.Product);

            var index = responseBody.IndexOf("-->", StringComparison.Ordinal);
            if (index > 0)
                responseBody = responseBody.Substring(index + 3);

            await this.CreateTextFile(this.Root.File(Name), responseBody);
        }

        private async Task<string> DownloadReadme(CancellationToken cancellationToken)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                _logger.LogInformation("Downloading {0} from {1}", this.Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download {Name}. The readme file will not be created", this.Name);
                return string.Empty;
            }
        }
    }
}
