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

        public override bool Equals(object obj)
        {
            return base.Equals(obj as ConfigurationKey);
        }

        public bool Equals(ConfigurationKey obj)
        {
            if (obj == null)
                return false;

            return this.Key.Equals(obj.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.Key.ToLowerInvariant().GetHashCode();
        }
    }
}
