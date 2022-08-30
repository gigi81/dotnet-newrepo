using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Abstractions;
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
            INewRepoSettings settings,
            ILogger<LicenseCreator> logger,
            IHttpClientFactory httpClientFactory)
            : base(settings, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string Url => $"https://raw.githubusercontent.com/spdx/license-list-data/master/text/{_settings.License}.txt";

        public override async Task Create(CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(_settings.License) || _settings.License.Equals("none", StringComparison.CurrentCultureIgnoreCase))
                return;

            using (var client = _httpClientFactory.CreateClient())
            {
                _logger.LogInformation("Downloading {0} from {1}", Name, this.Url);
                var response = await client.GetAsync(this.Url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                responseBody = responseBody.Replace("<year>", _settings.CopyrightYear);
                responseBody = responseBody.Replace("<copyright holders>", _settings.Authors);

                await this.CreateTextFile(this.Root.File(Name), responseBody);
            }
        }
    }
}
