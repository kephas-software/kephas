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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;

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
        private readonly ISettingsProviderSelector settingsProviderSelector;
        private TSettings settings;

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
        public TSettings Settings => this.settings ??= this.ComputeSettings();

        /// <summary>
        /// Updates the settings in the configuration store.
        /// </summary>
        /// <param name="settings">Optional. The settings to be updated. If no settings are provided, the current settings are used for the update.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation yielding an operation result
        /// with a true value in case of successful update and a false value if the settings could not be updated.
        /// </returns>
        public async Task<IOperationResult<bool>> UpdateSettingsAsync(
            TSettings? settings = null,
            CancellationToken cancellationToken = default)
        {
            if (settings == null && this.settings == null)
            {
                // settings not retrieved, no values to update.
                return false.ToOperationResult().MergeMessage("The settings are not changed, skipping update.");
            }

            settings ??= this.settings;
            var settingsProvider = this.settingsProviderSelector
                .TryGetProviders(typeof(TSettings))
                ?.FirstOrDefault();
            if (settingsProvider != null)
            {
                await settingsProvider.UpdateSettingsAsync(settings, cancellationToken).PreserveThreadContext();
                this.settings = settings;
                return true.ToOperationResult();
            }

            return false.ToOperationResult().MergeMessage($"No settings provider found for {typeof(TSettings)}.");
        }

        private TSettings ComputeSettings()
        {
            var settingsProviders = this.settingsProviderSelector.TryGetProviders(typeof(TSettings));
            if (settingsProviders != null)
            {
                foreach (var settingsProvider in settingsProviders)
                {
                    var settings = (TSettings?)settingsProvider.GetSettings(typeof(TSettings));
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