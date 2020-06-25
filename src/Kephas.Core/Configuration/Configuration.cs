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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Interaction;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Provides the configuration for the settings type indicated as the generic parameter type.
    /// </summary>
    /// <remarks>
    /// Being an <see cref="Expando"/>, various values may be added at runtime to this configuration.
    /// </remarks>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    public class Configuration<TSettings> : Expando, IConfiguration<TSettings>, IDisposable
        where TSettings : class, new()
    {
        private readonly ISettingsProviderSelector settingsProviderSelector;
        private readonly IAppRuntime appRuntime;
        private readonly Lazy<IEventHub> lazyEventHub;
        private TSettings? settings;
        private IEventSubscription? changeSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration{TSettings}"/> class.
        /// </summary>
        /// <param name="settingsProviderSelector">The settings provider selector.</param>
        /// <param name="appRuntime">Gets the application runtime.</param>
        /// <param name="lazyEventHub">The lazy event hub.</param>
        public Configuration(
            ISettingsProviderSelector settingsProviderSelector,
            IAppRuntime appRuntime,
            Lazy<IEventHub> lazyEventHub)
        {
            Requires.NotNull(settingsProviderSelector, nameof(settingsProviderSelector));
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(lazyEventHub, nameof(lazyEventHub));

            this.settingsProviderSelector = settingsProviderSelector;
            this.appRuntime = appRuntime;
            this.lazyEventHub = lazyEventHub;
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
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation yielding an operation result
        /// with a true value in case of successful update and a false value if the settings could not be updated.
        /// </returns>
        public async Task<IOperationResult<bool>> UpdateSettingsAsync(
            TSettings? settings = null,
            IContext? context = null,
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

                await this.lazyEventHub.Value.PublishAsync(
                    new ConfigurationChangedSignal(
                        $"Configuration of {typeof(TSettings).Name} changed by {this.appRuntime.GetAppInstanceId()}.")
                    {
                        SettingsType = typeof(TSettings),
                        SourceAppInstanceId = this.appRuntime.GetAppInstanceId(),
                    },
                    context!,
                    cancellationToken).PreserveThreadContext();
                return true.ToOperationResult();
            }

            return false.ToOperationResult().MergeMessage($"No settings provider found for {typeof(TSettings)}.");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.changeSubscription?.Dispose();
        }

        private TSettings ComputeSettings()
        {
            this.changeSubscription ??= this.lazyEventHub.Value.Subscribe<ConfigurationChangedSignal>(this.HandleConfigurationChangeAsync);
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

        private Task HandleConfigurationChangeAsync(ConfigurationChangedSignal signal, IContext context, CancellationToken cancellationToken)
        {
            if ((signal.SettingsType != null && signal.SettingsType != typeof(TSettings))
                    || signal.SourceAppInstanceId == this.appRuntime.GetAppInstanceId())
            {
                return Task.CompletedTask;
            }

            this.settings = default;
            return Task.CompletedTask;
        }
    }
}