// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A configuration returning no configuration values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// A configuration returning no configuration values.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullConfiguration : ConfigurationBase
    {
        /// <summary>
        /// Gets all available settings for the specified search pattern.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to search for (optional).</param>
        /// <returns>
        /// An enumeration of settings.
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, object>> GetSettingsCore(string searchPattern)
        {
            return new KeyValuePair<string, object>[0];
        }
    }
}