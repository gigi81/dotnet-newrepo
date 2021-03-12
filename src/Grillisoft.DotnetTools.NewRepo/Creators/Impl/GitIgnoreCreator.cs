using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class GitIgnoreCreator : CreatorBase
    {
        public string Name = ".gitignore";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly NewRepoOptions _options;
        private readonly ILogger<GitIgnoreCreator> _logger;

        public GitIgnoreCreator(
            NewRepoOptions options,
            IHttpClientFactory httpClientFactory,
            ILogger<GitIgnoreCreator> logger)
            : base(options)
        {
            _options = options;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string Url => "https://www.toptal.com/developers/gitignore/api/" + string.Join(',', _options.GitIgnoreTags);

        public override async Task Create(CancellationToken cancellationToken)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                _logger.LogInformation("Downloading {0} from {1}", Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                await this.CreateTextFile(this.Root.File(Name), responseBody, _logger);
            }
        }
    }
}
