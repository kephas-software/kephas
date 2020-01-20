﻿// --------------------------------------------------------------------------------------------------------------------
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
        where TSettings : class, new()
    {
        private TSettings settings;
        private ISettingsProviderSelector settingsProviderSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration{TSettings}"/> class.
        /// </summary>
        /// <param name="settingsProviderSelector">The settings provider selector.</param>
        public Configuration(ISettingsProviderSelector settingsProviderSelector)
        {
            Requires.NotNull(settingsProviderSelector, nameof(settingsProviderSelector));

            this.settingsProviderSelector = settingsProviderSelector;
        }

        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public TSettings Settings => this.settings ?? (this.settings = this.ComputeSettings());

        private TSettings ComputeSettings()
        {
            var settingsProviders = this.settingsProviderSelector.TryGetProviders(typeof(TSettings));
            if (settingsProviders != null)
            {
                foreach (var settingsProvider in settingsProviders)
                {
                    var settings = (TSettings)settingsProvider.GetSettings(typeof(TSettings));
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }

            return new TSettings();
        }
    }
}