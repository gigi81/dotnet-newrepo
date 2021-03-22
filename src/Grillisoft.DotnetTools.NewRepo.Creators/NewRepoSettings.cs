using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo
{
    public sealed class NewRepoSettings : INewRepoSettings
    {
        public const string InitFilename = "init.json";

        private readonly DirectoryInfo _root;
        private IDictionary<ConfigurationKey, object> _values;

        public NewRepoSettings()
        {
            _root = new DirectoryInfo(".");
            _values = ConfigurationKeysManager.Keys.Values.ToDictionary(k => k, k => k.DefaultValue);
        }

        public NewRepoSettings(string[] args)
        {
            _root = new DirectoryInfo(args.Length > 0 ? args[0] : ".");
            _values = ConfigurationKeysManager.Keys.Values.ToDictionary(k => k, k => k.DefaultValue);
        }

        public DirectoryInfo Root => _root;

        public FileInfo InitFile => _root.File(InitFilename);

        public T Get<T>(ConfigurationKey key)
        {
            throw new NotImplementedException();
        }

        public bool GetBool(ConfigurationKey key)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(ConfigurationKey key)
        {
            throw new NotImplementedException();
        }

        public string GetString(ConfigurationKey key)
        {
            throw new NotImplementedException();
        }

        public Task Init(ILogger logger, CancellationToken cancellationToken)
        {
            return this.InitFile.WriteAllLinesAsync(GetInitContent(), cancellationToken);
        }

        private IEnumerable<string> GetInitContent()
        {
            yield return "{";

            foreach (var value in _values)
            {
                yield return $"  //{value.Key.Help}";
                yield return $"  \"{value.Key.Key}\": " + JsonSerializer.Serialize(value.Value);
            }

            yield return "}";
        }

        public async Task Load(ILogger logger, CancellationToken token)
        {
            var init = this.Root.File(InitFilename);
            if (!init.Exists)
            {
                logger.LogWarning($"Settings file {InitFilename} not found. Will use default settings");
                return;
            }

            try
            {
                logger.LogInformation("Loading settings from {0}", init.FullName);
                using (var stream = init.OpenRead())
                    _values = (await JsonSerializer.DeserializeAsync<IDictionary<string, object>>(stream, null, token))
                                .ToDictionary(k => ConfigurationKeysManager.Keys[k.Key], k => k.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load settings from {init.FullName}: {ex.Message}", ex);
            }
        }

        public bool TryGet<T>(ConfigurationKey key, out T value)
        {
            throw new NotImplementedException();
        }
    }
}
