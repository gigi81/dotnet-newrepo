using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class LicenseCreator : CreatorBase
    {
        public string Name = "LICENSE.md";

        private readonly IHttpClientFactory _httpClientFactory;

        public LicenseCreator(
            NewRepoSettings settings,
            ILogger<LicenseCreator> logger,
            IHttpClientFactory httpClientFactory)
            : base(settings, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string Url => $"https://raw.githubusercontent.com/spdx/license-list-data/master/text/{_settings.License}.txt";

        public override async Task Create(CancellationToken cancellationToken)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                _logger.LogInformation("Downloading {0} from {1}", Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                responseBody = responseBody.Replace("<year>", _settings.CopyrightYear);
                responseBody = responseBody.Replace("<copyright holders>", _settings.CopyrightHolders);

                await this.CreateTextFile(this.Root.File(Name), responseBody);
            }
        }
    }
}
