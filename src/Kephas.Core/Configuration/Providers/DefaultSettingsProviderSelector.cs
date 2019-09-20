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

    using Kephas.Composition;
    using Kephas.Configuration.Providers.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Text;

    /// <summary>
    /// A default settings provider selector.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultSettingsProviderSelector : ISettingsProviderSelector
    {
        private readonly IOrderedServiceCollection<ISettingsProvider, SettingsProviderMetadata> providerFactories;
        private readonly ConcurrentDictionary<Type, ISettingsProvider> providersMap = new ConcurrentDictionary<Type, ISettingsProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettingsProviderSelector"/> class.
        /// </summary>
        /// <param name="providerFactories">The provider factories.</param>
        public DefaultSettingsProviderSelector(ICollection<IExportFactory<ISettingsProvider, SettingsProviderMetadata>> providerFactories)
        {
            Requires.NotNull(providerFactories, nameof(providerFactories));

            this.providerFactories = providerFactories.Order();
        }

        /// <summary>
        /// Gets the provider handling a specific settings type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The provider.
        /// </returns>
        public ISettingsProvider GetProvider(Type settingsType)
        {
            return this.providersMap.GetOrAdd(settingsType, _ => this.ComputeConfigurationProvider(settingsType));
        }

        /// <summary>
        /// Calculates the settings provider.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The calculated settings provider.
        /// </returns>
        private ISettingsProvider ComputeConfigurationProvider(Type settingsType)
        {
            var orderedFactories = this.providerFactories;

            var factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType == settingsType);
            if (factory == null)
            {
                factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType?.IsAssignableFrom(settingsType) ?? false);
                if (factory == null)
                {
                    factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType == null);
                }
            }

            if (factory == null)
            {
                // TODO provide a more explicit exception information.
                throw new NotSupportedException(Strings.SettingsProviderSelector_NoProviderFoundForSettingsType.FormatWith(settingsType));
            }

            return factory.CreateExportedValue();
        }
    }
}
