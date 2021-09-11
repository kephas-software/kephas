// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationAppSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Configuration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Configuration.Providers;
    using Kephas.Services;

    /// <summary>
    /// Settings provider for <see cref="AppSettings"/> retrieving them from the <see cref="OrchestrationSettings"/>
    /// for the current application instance.
    /// </summary>
    [OverridePriority(Priority.AboveNormal)]
    [SettingsType(typeof(AppSettings))]
    public class OrchestrationAppSettingsProvider : ISettingsProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly Lazy<IConfiguration<OrchestrationSettings>> lazyConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="lazyConfiguration">The lazy configuration for orchestration.</param>
        public OrchestrationAppSettingsProvider(IAppRuntime appRuntime, Lazy<IConfiguration<OrchestrationSettings>> lazyConfiguration)
        {
            this.appRuntime = appRuntime;
            this.lazyConfiguration = lazyConfiguration;
        }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public object? GetSettings(Type settingsType, IContext? context)
        {
            var appId = this.appRuntime.GetAppId()!;
            var orchestrationSettings = this.lazyConfiguration.Value.GetSettings();
            return orchestrationSettings.Instances.TryGetValue(appId);
        }

        /// <summary>
        /// Updates the settings asynchronously.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task UpdateSettingsAsync(object settings, IContext? context, CancellationToken cancellationToken = default)
        {
            var appId = this.appRuntime.GetAppId()!;
            var orchestrationSettings = this.lazyConfiguration.Value.GetSettings();
            orchestrationSettings.Instances[appId] = (AppSettings)settings;
            return this.lazyConfiguration.Value.UpdateSettingsAsync(orchestrationSettings, context, cancellationToken);
        }
    }
}