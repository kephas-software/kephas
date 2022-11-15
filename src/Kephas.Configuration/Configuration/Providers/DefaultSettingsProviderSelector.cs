// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSettingsProviderSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default settings provider selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A default settings provider selector.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultSettingsProviderSelector : Loggable, ISettingsProviderSelector
    {
        private readonly IOrderedServiceFactoryCollection<ISettingsProvider, SettingsProviderMetadata> providerFactories;
        private readonly ConcurrentDictionary<Type, IEnumerable<ISettingsProvider>> providersMap = new ConcurrentDictionary<Type, IEnumerable<ISettingsProvider>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettingsProviderSelector"/> class.
        /// </summary>
        /// <param name="providerFactories">The provider factories.</param>
        public DefaultSettingsProviderSelector(ICollection<IExportFactory<ISettingsProvider, SettingsProviderMetadata>> providerFactories)
        {
            providerFactories = providerFactories ?? throw new ArgumentNullException(nameof(providerFactories));

            this.providerFactories = providerFactories.Order();
        }

        /// <summary>
        /// Gets the provider handling a specific settings type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The provider.
        /// </returns>
        public IEnumerable<ISettingsProvider> TryGetProviders(Type settingsType)
        {
            return this.providersMap.GetOrAdd(settingsType, _ => this.ComputeConfigurationProviders(settingsType).ToArray());
        }

        /// <summary>
        /// Calculates the settings providers.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The calculated settings provider.
        /// </returns>
        private IEnumerable<ISettingsProvider> ComputeConfigurationProviders(Type settingsType)
        {
            var orderedFactories = this.providerFactories;

            var exists = false;
            foreach (var factory in orderedFactories.Where(f => f.Metadata.SettingsType == settingsType))
            {
                exists = true;
                yield return factory.CreateExportedValue();
            }

            foreach (var factory in orderedFactories.Where(f => f.Metadata.SettingsType?.IsAssignableFrom(settingsType) ?? false))
            {
                exists = true;
                yield return factory.CreateExportedValue();
            }

            foreach (var factory in orderedFactories.Where(f => f.Metadata.SettingsType == null))
            {
                exists = true;
                yield return factory.CreateExportedValue();
            }

            if (!exists)
            {
                this.Logger.Warn(AbstractionStrings.SettingsProviderSelector_NoProviderFoundForSettingsType, settingsType);
            }
        }
    }
}
