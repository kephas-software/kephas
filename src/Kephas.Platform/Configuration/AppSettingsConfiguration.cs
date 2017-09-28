// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application settings configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Configuration based on the AppSettings found in the app.config/web.config file.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class AppSettingsConfiguration : AppConfigurationBase
    {
        /// <summary>
        /// The application settings.
        /// </summary>
        private readonly IDictionary<string, object> appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsConfiguration"/> class.
        /// </summary>
        public AppSettingsConfiguration()
        {
            var configAppSettings = ConfigurationManager.AppSettings;
            this.appSettings = configAppSettings.AllKeys.ToDictionary(k => k, k => (object)configAppSettings[k]);
        }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public override object this[string key]
        {
            get => base[key];
            set => this.appSettings[key] = value;
        }

        /// <summary>
        /// Gets all available settings for the specified search pattern.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to search for (optional).</param>
        /// <returns>
        /// An enumeration of settings.
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, object>> GetSettingsCore(string searchPattern)
        {
            IEnumerable<KeyValuePair<string, object>> sectionSettings;
            if (TryGetSectionSettings(searchPattern, out sectionSettings))
            {
                return sectionSettings;
            }

            if (searchPattern.EndsWith("*"))
            {
                var start = searchPattern.Substring(0, searchPattern.Length - 1);
                var keys = this.appSettings.Where(kv => kv.Key.StartsWith(start));
                return keys;
            }

            object setting;
            this.appSettings.TryGetValue(searchPattern, out setting);
            return new[] { new KeyValuePair<string, object>(searchPattern, setting) };
        }

        /// <summary>
        /// Attempts to get section settings from the given data.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to search for (optional).</param>
        /// <param name="sectionSettings">The section settings.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private static bool TryGetSectionSettings(string searchPattern, out IEnumerable<KeyValuePair<string, object>> sectionSettings)
        {
            if (!searchPattern.StartsWith(":"))
            {
                sectionSettings = null;
                return false;
            }

            string section;
            var nextSectionSeparatorIndex = searchPattern.IndexOf(":", 1);
            if (nextSectionSeparatorIndex > 0)
            {
                section = searchPattern.Substring(1, nextSectionSeparatorIndex - 1);
                searchPattern = searchPattern.Substring(nextSectionSeparatorIndex + 1);
            }
            else
            {
                section = searchPattern.Substring(1);
                searchPattern = "*";
            }

            var isWildCard = string.IsNullOrEmpty(searchPattern) || searchPattern.EndsWith("*");
            if (searchPattern.EndsWith("*"))
            {
                searchPattern = searchPattern.Substring(0, searchPattern.Length - 1);
            }

            var settings = (NameValueCollection)ConfigurationManager.GetSection(section);
            if (settings == null)
            {
                sectionSettings = null;
                return false;
            }

            if (!isWildCard)
            {
                object setting = settings[searchPattern];
                sectionSettings = new[] { new KeyValuePair<string, object>(searchPattern, setting) };
                return true;
            }

            var settingsDictionary = new Dictionary<string, object>();
            foreach (var key in settings.AllKeys)
            {
                if (key.StartsWith(searchPattern))
                {
                    settingsDictionary[key] = settings[key];
                }
            }

            sectionSettings = settingsDictionary;
            return true;
        }
    }
}