// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConfigurationStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default configuration store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Dynamic;

    /// <summary>
    /// A default configuration store.
    /// </summary>
    public class DefaultConfigurationStore : Expando, IConfigurationStore
    {
        private IDictionary<Type, object> settingsMap = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationStore"/> class.
        /// </summary>
        public DefaultConfigurationStore()
            : base(new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase))
        {
        }

        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="optionsConfig">The options configuration.</param>
        public void Configure<TSettings>(Action<TSettings> optionsConfig)
            where TSettings : class, new()
        {
            if (!this.settingsMap.TryGetValue(typeof(TSettings), out var settings))
            {
                settings = new TSettings();
                this.settingsMap.Add(typeof(TSettings), settings);
            }

            optionsConfig?.Invoke((TSettings)settings);
        }

        /// <summary>
        /// Tries to get the indicated settings.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The required settings or <c>null</c>.
        /// </returns>
        public object TryGetSettings(Type settingsType)
        {
            if (settingsType == null)
            {
                return null;
            }

            return this.settingsMap.TryGetValue(settingsType);
        }
    }
}