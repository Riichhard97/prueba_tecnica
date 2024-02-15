using System;
using Nexu.Shared;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationSectionExtensions
    {
        private static readonly object Empty = new object();

        public static IConfigurationSection GetRequiredSection(this IConfiguration configuration, string key)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var section = configuration.GetSection(key);
            if (!section.Exists())
            {
                throw new AppConfigurationException($"Configuration section '{GetPath(configuration, key)}' not found.");
            }

            return section;
        }

        public static string GetRequiredValue(this IConfiguration configuration, string key, bool allowEmpty = false)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = configuration[key];

            if (value is null)
            {
                throw new AppConfigurationException($"Configuration key '{GetPath(configuration, key)}' not present.");
            }

            if (string.IsNullOrEmpty(value) && !allowEmpty)
            {
                throw new AppConfigurationException($"Configuration key '{GetPath(configuration, key)}' is empty.");
            }

            return value;
        }

        public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // This will throw if the value does not exist
            GetRequiredSection(configuration, key);

            var value = configuration.GetValue(typeof(T), key, Empty);
            if (value == Empty)
            {
                throw new AppConfigurationException($"Configuration key '{GetPath(configuration, key)}' not present.");
            }

            return (T)value;
        }

        private static string GetPath(IConfiguration configuration, string key)
        {
            if (configuration is IConfigurationSection section)
            {
                return section.Path + ConfigurationPath.KeyDelimiter + key;
            }

            return key;
        }
    }
}
