using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoteMyst.Database
{
    public class Configuration
    {
        public string this[string key]
            => GetValue(key);

        private readonly VoteMystContext _databaseContext;

        private const StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;

        public Configuration(VoteMystContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool HasValue(string key)
            => GetConfigurationValue(key) != null;

        public string GetValue(string key)
            => GetConfigurationValue(key)?.Value ?? throw new KeyNotFoundException();
        public string GetValue(string key, string defaultValue)
            => GetConfigurationValue(key)?.Value ?? defaultValue;

        public bool SetValue(string key, string value)
        {
            ConfigurationValue configurationValue = GetConfigurationValue(key);

            if (configurationValue != null)
            {
                configurationValue.Value = value;
            }
            else
            {
                configurationValue = new ConfigurationValue { Key = key, Value = value };

                _databaseContext.Configuration.Add(configurationValue);
            }

            return _databaseContext.SaveChanges() > 0;
        }

        private ConfigurationValue GetConfigurationValue(string key)
            => _databaseContext.Configuration.FirstOrDefault(c => c.Key.Equals(key, _comparison));
    }
}
