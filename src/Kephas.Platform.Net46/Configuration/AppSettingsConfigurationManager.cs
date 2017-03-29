// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsConfigurationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application settings configuration manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Configuration manager based on the AppSettings found in the app.config/web.config file.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class AppSettingsConfigurationManager : ConfigurationManagerBase
    {
        /// <summary>
        /// The application settings.
        /// </summary>
        private readonly IDictionary<string, object> appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsConfigurationManager"/> class.
        /// </summary>
        public AppSettingsConfigurationManager()
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
            get
            {
                return base[key];
            }
            set
            {
                this.appSettings[key] = value;
            }
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
            if (searchPattern[searchPattern.Length - 1] == '*')
            {
                var start = searchPattern.Substring(0, searchPattern.Length - 1);
                var keys = this.appSettings.Where(kv => kv.Key.StartsWith(start));
                return keys;
            }

            return new[] { new KeyValuePair<string, object>(searchPattern, this.appSettings[searchPattern]) };
        }
    }
}