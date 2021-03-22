using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
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
            using (var client = _httpClientFactory.CreateClient())
            {
                _logger.LogInformation("Downloading {0} from {1}", Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                await this.CreateTextFile(this.Root.File(Name), responseBody);
            }
        }
    }
}
