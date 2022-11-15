// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationStartupAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Application.Interaction;
    using Kephas.Configuration;
    using Kephas.Interaction;
    using Kephas.Model.AttributedModel;
    using Kephas.Orchestration.Configuration;
    using Kephas.Runtime;

    /// <summary>
    /// Application service for scheduling startup commands in a microservices deployment.
    /// </summary>
    [Override]
    public class OrchestrationStartupAppLifecycleBehavior : StartupAppLifecycleBehavior
    {
        private readonly IConfiguration<OrchestrationSettings> orchestrationConfiguration;
        private readonly Lazy<IOrchestrationManager> lazyOrchestrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationStartupAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="orchestrationConfiguration">The orchestration configuration.</param>
        /// <param name="lazyOrchestrator">The lazy orchestration manager.</param>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="appContext">The application context.</param>
        public OrchestrationStartupAppLifecycleBehavior(
            IEventHub eventHub,
            IConfiguration<AppSettings> appConfiguration,
            IConfiguration<OrchestrationSettings> orchestrationConfiguration,
            Lazy<IOrchestrationManager> lazyOrchestrator,
            IRuntimeTypeRegistry typeRegistry,
            IAppContext appContext)
        : base(eventHub, appConfiguration, typeRegistry, appContext)
        {
            this.orchestrationConfiguration = orchestrationConfiguration;
            this.lazyOrchestrator = lazyOrchestrator;
        }

        /// <summary>
        /// Gets the application settings where the commands should be persisted.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="appContext">The application context.</param>
        /// <returns>The application settings.</returns>
        protected override AppSettings GetAppSettings(ScheduleStartupCommandSignal signal, IAppContext appContext)
        {
            var appId = this.GetAppId(signal);
            var settings = this.orchestrationConfiguration.GetSettings(appContext);
            if (!settings.Instances.TryGetValue(appId, out var appSettings))
            {
                settings.Instances[appId] = appSettings = new AppSettings();
            }

            return appSettings;
        }

        /// <summary>
        /// Updates the application settings for which the startup commands were added.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected override Task UpdateSettingsAsync(AppSettings appSettings, IAppContext appContext, CancellationToken cancellationToken)
        {
            return this.orchestrationConfiguration.UpdateSettingsAsync(context: appContext, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets the ID of the application where the command should be persisted.
        /// If no application ID is provided, the root is considered.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The ID of the application.</returns>
        protected virtual string GetAppId(ScheduleStartupCommandSignal signal)
        {
            var appId = signal.AppId;
            if (string.IsNullOrEmpty(appId))
            {
                appId = this.lazyOrchestrator.Value.GetRootAppInstanceId();
            }

            return appId;
        }
    }
}