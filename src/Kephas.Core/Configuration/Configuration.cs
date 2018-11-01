// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Configuration.Composition;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides the configuration for the settings type indicated as the generic parameter type.
    /// </summary>
    /// <remarks>
    /// Being an <see cref="Expando"/>, various values may be added at runtime to this configuration.
    /// </remarks>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    public class Configuration<TSettings> : Expando, IConfiguration<TSettings>
        where TSettings : class
    {
        /// <summary>
        /// The provider factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>> providerFactories;

        /// <summary>
        /// The settings.
        /// </summary>
        private TSettings settings;

        /// <summary>
        /// The configuration provider.
        /// </summary>
        private IConfigurationProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration{TSettings}"/> class.
        /// </summary>
        /// <param name="providerFactories">The provider factories.</param>
        public Configuration(ICollection<IExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>> providerFactories)
        {
            Requires.NotNull(providerFactories, nameof(providerFactories));

            this.providerFactories = providerFactories;
        }

        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public TSettings Settings => this.settings ?? (this.settings = this.ComputeSettings());

        /// <summary>
        /// Gets or sets the configuration provider.
        /// </summary>
        /// <value>
        /// The configuration provider.
        /// </value>
        public IConfigurationProvider Provider
        {
            get => this.provider ?? (this.provider = this.ComputeConfigurationProvider());
        }

        /// <summary>
        /// Calculates the settings.
        /// </summary>
        /// <returns>
        /// The calculated settings.
        /// </returns>
        private TSettings ComputeSettings()
        {
            return (TSettings)this.Provider.GetSettings(typeof(TSettings));
        }

        /// <summary>
        /// Calculates the configuration provider.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <returns>
        /// The calculated configuration provider.
        /// </returns>
        private IConfigurationProvider ComputeConfigurationProvider()
        {
            var orderedFactories = this.providerFactories
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ToList();

            var factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType == typeof(TSettings));
            if (factory == null)
            {
                factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType?.IsAssignableFrom(typeof(TSettings)) ?? false);
                if (factory == null)
                {
                    factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType == null);
                }
            }

            if (factory == null)
            {
                // TODO provide a more explicit exception information.
                throw new NotSupportedException($"No provider found for settings type {typeof(TSettings)}.");
            }

            return factory.CreateExportedValue();
        }
    }
}