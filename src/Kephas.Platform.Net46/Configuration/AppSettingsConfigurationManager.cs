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
    [OverridePriority(Priority.Low)]
    public class AppSettingsConfigurationManager : ConfigurationManagerBase
    {
        /// <summary>
        /// Gets all available settings for the specified search pattern.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to search for (optional).</param>
        /// <returns>
        /// An enumeration of settings.
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, object>> GetSettings(string searchPattern)
        {
            var appSettings = ConfigurationManager.AppSettings;
            if (searchPattern[searchPattern.Length - 1] == '*')
            {
                var start = searchPattern.Substring(0, searchPattern.Length - 1);
                var keys = appSettings.AllKeys.Where(k => k.StartsWith(start));
                return keys.Select(k => new KeyValuePair<string, object>(k, appSettings.Get(k)));
            }

            return new[] { new KeyValuePair<string, object>(searchPattern, appSettings.Get(searchPattern)) };
        }
    }
}