using System;

namespace Grillisoft.DotnetTools.NewRepo.Abstractions
{
    public class ConfigurationKey
    {
        public ConfigurationKey()
        {
        }

        public ConfigurationKey(string key, Type type, string help, object defaultValue)
        {
            this.Key = key;
            this.Type = type;
            this.Help = help;
            this.DefaultValue = defaultValue;
        }

        public string Key { get; init; }
        public Type Type { get; init; }
        public string Help { get; init; }
        public object DefaultValue { get; init; }
    }
}
