using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Grillisoft.DotnetTools.NewRepo.Configuration.Yaml
{
    public sealed class YamlNewRepoSettings : INewRepoSettings
    {
        private const string InitFilename = "init.yml";

        private readonly DirectoryInfo _root;
        private IDictionary<ConfigurationKey, object> _values;

        private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

        private static readonly ISerializer YamlSerializer = new SerializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

        public YamlNewRepoSettings()
        {
            _root = new DirectoryInfo(".");
            _values = ConfigurationKeysManager.Keys.Values.ToDictionary(k => k, k => k.DefaultValue);
        }

        public YamlNewRepoSettings(string[] args)
        {
            _root = new DirectoryInfo(args.Length > 0 ? args[0] : ".");
            _values = ConfigurationKeysManager.Keys.Values.ToDictionary(k => k, k => k.DefaultValue);
        }

        public DirectoryInfo Root => _root;

        public FileInfo InitFile => _root.File(InitFilename);

        public T Get<T>(ConfigurationKey key)
        {
            return (T)_values[key];
        }

        public bool GetBool(ConfigurationKey key)
        {
            return (bool)_values[key];
        }

        public int GetInt32(ConfigurationKey key)
        {
            return (int)_values[key];
        }

        public string GetString(ConfigurationKey key)
        {
            return (string)_values[key];
        }

        public bool TryGet<T>(ConfigurationKey key, out T value)
        {
            if (!_values.TryGetValue(key, out var tmp))
            {
                value = default(T);
                return false;
            }

            value = (T)tmp;
            return true;
        }

        public Task Init(ILogger logger, CancellationToken cancellationToken)
        {
            return this.InitFile.WriteAllLinesAsync(GetInitContent());
        }

        private IEnumerable<string> GetInitContent()
        {
            foreach (var value in _values)
            {
                yield return $"# {value.Key.Help}";
                //TODO: improve this, maybe implement a custom INodeDeserializer for KeyValuePair https://github.com/aaubry/YamlDotNet/issues/249
                yield return YamlSerializer.Serialize(new Dictionary<string, object>(new[] { new KeyValuePair<string, object>(value.Key.Key, value.Value) }));
            }
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
                using (var reader = new StreamReader(stream))
                {
                    var ret = await Task.Run(() => YamlDeserializer.Deserialize<Dictionary<string, object>>(reader));
                    _values = ret.ToDictionary(k => ConfigurationKeysManager.Keys[k.Key], k => GetValue(k.Value, ConfigurationKeysManager.Keys[k.Key]));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load settings from {init.FullName}: {ex.Message}", ex);
            }
        }

        private object GetValue(object value, ConfigurationKey key)
        {
            try
            {
                if(!key.Type.IsEnumerable(out var itemType))
                    return Convert.ChangeType(value, key.Type);

                var ret = CreateList(itemType);
                foreach (var item in (IEnumerable)value)
                    ret.Add(Convert.ChangeType(item, itemType));

                var array = Array.CreateInstance(itemType, ret.Count);
                for (int i = 0; i < ret.Count; i++)
                    array.SetValue(Convert.ChangeType(ret[i], itemType), i);

                return array;
            }
            catch(Exception ex)
            {
                Console.WriteLine(key.Key);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);

                throw;
            }
        }

        private static IList CreateList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }
    }
}
