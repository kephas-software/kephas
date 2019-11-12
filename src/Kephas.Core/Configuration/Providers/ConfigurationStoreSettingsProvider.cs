// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationStoreSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration store settings provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A configuration store settings provider.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class ConfigurationStoreSettingsProvider : ISettingsProvider
    {
        private readonly IConfigurationStore configurationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStoreSettingsProvider"/> class.
        /// </summary>
        /// <param name="configurationStore">The configuration store.</param>
        public ConfigurationStoreSettingsProvider(IConfigurationStore configurationStore)
        {
            Requires.NotNull(configurationStore, nameof(configurationStore));

            this.configurationStore = configurationStore;
        }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public object GetSettings(Type settingsType)
        {
            Requires.NotNull(settingsType, nameof(settingsType));

            return this.configurationStore.TryGetSettings(settingsType);
        }
    }
}
