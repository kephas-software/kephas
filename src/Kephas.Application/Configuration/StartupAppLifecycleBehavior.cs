﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Interaction;
    using Kephas.Configuration;
    using Kephas.Interaction;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Singleton application service for scheduling
    /// </summary>
    public class StartupAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IEventHub eventHub;
        private readonly IConfiguration<SystemSettings> systemConfiguration;
        private IEventSubscription? scheduleCommandSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        public StartupAppLifecycleBehavior(IEventHub eventHub, IConfiguration<SystemSettings> systemConfiguration)
        {
            this.eventHub = eventHub;
            this.systemConfiguration = systemConfiguration;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.scheduleCommandSubscription = this.eventHub.Subscribe<ScheduleStartupCommandSignal>((signal, ctx, token) =>
                this.HandleScheduleStartupCommandSignalAsync(signal, appContext, token));
            return base.BeforeAppInitializeAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.scheduleCommandSubscription?.Dispose();
            this.scheduleCommandSubscription = null;
            return base.AfterAppFinalizeAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Gets the ID of the application where the command should be persisted.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The ID of the application.</returns>
        protected virtual string? GetAppId(ScheduleStartupCommandSignal signal)
        {
            return signal.AppId;
        }

        private async Task HandleScheduleStartupCommandSignalAsync(ScheduleStartupCommandSignal signal, IAppContext appContext, CancellationToken token)
        {
            var settings = this.systemConfiguration.Settings;
            settings.Instances ??= new Dictionary<string, AppSettings>();
            var appId = this.GetAppId(signal);
            if (string.IsNullOrEmpty(appId))
            {
                var commands = settings.SetupCommands == null
                    ? new List<object>()
                    : new List<object>(settings.SetupCommands);
                commands.Add(signal.Command);
                settings.SetupCommands = commands.ToArray();
            }
            else
            {
                if (!settings.Instances.TryGetValue(appId, out var appSettings))
                {
                    settings.Instances[appId] = appSettings = new AppSettings();
                }

                var commands = appSettings.SetupCommands == null
                    ? new List<object>()
                    : new List<object>(appSettings.SetupCommands);
                commands.Add(signal.Command);
                appSettings.SetupCommands = commands.ToArray();
            }

            await this.systemConfiguration.UpdateSettingsAsync(cancellationToken: token).PreserveThreadContext();
        }
    }
}