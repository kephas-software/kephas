// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Interaction;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
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
        private IEventSubscription? configChangedSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public ConfigurationAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IEventHub eventHub,
            IMessageBroker messageBroker,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
            this.messageBroker = messageBroker;
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
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
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
        public override async Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            await base.AfterAppFinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
            this.configChangedSubscription?.Dispose();
        }

        /// <summary>
        /// Handles the <see cref="ConfigurationChangedSignal"/>.
        /// </summary>
        /// <param name="signal">The signal to handle.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual async Task HandleConfigurationChangedAsync(ConfigurationChangedSignal signal, IContext context, CancellationToken cancellationToken)
        {
            if (signal.SourceAppInstanceId != this.appRuntime.GetAppInstanceId())
            {
                return;
            }

            this.Logger.Info("Notify other peers that the configuration for {settingsType} changed.", signal.SettingsType);

            // notify the other peers (and myself, for now) that the configuration has changed
            await this.messageBroker.PublishAsync(
                    signal,
                    ctx => ctx.Impersonate(context),
                    cancellationToken)
                .PreserveThreadContext();
        }
    }
}