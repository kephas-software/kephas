// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic settings provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Provider retrieving the settings from the configuration store.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class StoreSettingsProvider : ISettingsProvider
    {
        private readonly IConfigurationStore store;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingsProvider"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public StoreSettingsProvider(IConfigurationStore store)
        {
            this.store = store;
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
            return this.store.TryGetSettings(settingsType);
        }
    }
}
