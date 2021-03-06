﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Interaction;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application lifecycle behavior for configuration.
    /// </summary>
    public class ConfigurationAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;
        private readonly IMessageBroker messageBroker;
        private readonly Lazy<IOrchestrationManager> lazyOrchestrationManager;
        private IEventSubscription? configChangedSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="lazyOrchestrationManager">The lazy orchestration manager.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public ConfigurationAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IEventHub eventHub,
            IMessageBroker messageBroker,
            Lazy<IOrchestrationManager> lazyOrchestrationManager,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
            this.messageBroker = messageBroker;
            this.lazyOrchestrationManager = lazyOrchestrationManager;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        public override Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.configChangedSubscription = this.eventHub.Subscribe<ConfigurationChangedSignal>(this.HandleConfigurationChangedAsync);
            return base.BeforeAppInitializeAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override async Task<IOperationResult> AfterAppFinalizeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            var baseResult = await base.AfterAppFinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
            this.configChangedSubscription?.Dispose();
            return baseResult;
        }

        /// <summary>
        /// Handles the <see cref="ConfigurationChangedSignal"/>.
        /// </summary>
        /// <param name="signal">The signal to handle.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual async Task HandleConfigurationChangedAsync(ConfigurationChangedSignal signal, IContext? context, CancellationToken cancellationToken)
        {
            var thisAppInstanceId = this.appRuntime.GetAppInstanceId();
            var sourceAppInstanceId = signal.SourceAppInstanceId;
            if (sourceAppInstanceId != thisAppInstanceId)
            {
                return;
            }

            var liveApps = await this.lazyOrchestrationManager.Value
                .GetLiveAppsAsync(ctx => ctx.Impersonate(context), cancellationToken)
                .PreserveThreadContext();
            var targetAppInstanceIds = liveApps.Select(a => a.AppInstanceId)
                .Where(iid => iid != thisAppInstanceId && iid != sourceAppInstanceId)
                .ToList();
            if (targetAppInstanceIds.Count == 0)
            {
                return;
            }

            var recipients = targetAppInstanceIds
                .Select(iid => new Endpoint(appInstanceId: iid));

            this.Logger.Info("Notify peers {peers} that the configuration for {settingsType} changed.", targetAppInstanceIds, signal.SettingsType);

            // notify the other peers that the configuration has changed
            await this.messageBroker.PublishAsync(
                    signal,
                    ctx => ctx
                        .Impersonate(context)
                        .To(recipients),
                    cancellationToken)
                .PreserveThreadContext();
        }
    }
}