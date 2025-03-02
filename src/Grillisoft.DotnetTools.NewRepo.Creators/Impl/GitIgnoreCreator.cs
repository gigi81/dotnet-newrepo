using System;
using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class GitIgnoreCreator : CreatorBase
    {
        public string Name = ".gitignore";

        private readonly IHttpClientFactory _httpClientFactory;

        public GitIgnoreCreator(
            INewRepoSettings settings,
            IHttpClientFactory httpClientFactory,
            ILogger<GitIgnoreCreator> logger)
            : base(settings, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string Url => "https://www.toptal.com/developers/gitignore/api/" + string.Join(',', _settings.GitIgnoreTags);

        public override async Task Create(CancellationToken cancellationToken)
        {
            var responseBody = await DownloadGitIgnore(cancellationToken);

            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                await this.CreateTextFile(this.Root.File(Name), responseBody);                
            }
            else
            {
                _logger.LogWarning("Creating gitignore using dotnet command as fallback");
                await Run("dotnet", "new .gitignore", cancellationToken);
            }
        }

        private async Task<string> DownloadGitIgnore(CancellationToken cancellationToken)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                _logger.LogInformation("Downloading {0} from {1}", Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download {0} from {1}", this.Name, this.Url);
                return string.Empty;
            }
        }
    }
}
