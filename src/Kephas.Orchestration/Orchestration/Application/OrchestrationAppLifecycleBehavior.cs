// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Operations;
    using Kephas.Orchestration.Interaction;
    using Kephas.Orchestration.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An orchestration application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class OrchestrationAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IAppRuntime appRuntime;
        private readonly IAppContext appContext;
        private readonly IMessageBroker messageBroker;
        private readonly IMessageHandlerRegistry messageHandlerRegistry;
        private readonly IEventHub eventHub;
        private readonly IHostInfoProvider hostInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="appContext">The application context.</param>
        /// <param name="messageBroker">The application event publisher.</param>
        /// <param name="messageHandlerRegistry">The message handler registry.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="hostInfoProvider">The host information provider.</param>
        /// <param name="logger">Optional. The logger.</param>
        public OrchestrationAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IAppContext appContext,
            IMessageBroker messageBroker,
            IMessageHandlerRegistry messageHandlerRegistry,
            IEventHub eventHub,
            IHostInfoProvider hostInfoProvider,
            ILogger<OrchestrationAppLifecycleBehavior>? logger = null)
        {
            this.Logger = logger;
            this.appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
            this.appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            this.messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            this.messageHandlerRegistry = messageHandlerRegistry ?? throw new ArgumentNullException(nameof(messageHandlerRegistry));
            this.eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            this.hostInfoProvider = hostInfoProvider ?? throw new ArgumentNullException(nameof(hostInfoProvider));

            this.messageHandlerRegistry.RegisterHandler<AppStoppedEvent>(this.HandleAppStoppedEventAsync);
            this.messageHandlerRegistry.RegisterHandler<AppStoppingEvent>(this.HandleAppStoppingEventAsync);
            this.messageHandlerRegistry.RegisterHandler<AppStartedEvent>(this.HandleAppStartedEventAsync);
            this.messageHandlerRegistry.RegisterHandler<AppStartingEvent>(this.HandleAppStartingEventAsync);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger<OrchestrationAppLifecycleBehavior>? Logger { get; }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        public async Task<IOperationResult> BeforeAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStartingEvent = this.CreateAppStartingEvent();

            try
            {
                // notify its own manager that it is starting.
                await this.eventHub.PublishAsync(appStartingEvent, this.appContext, cancellationToken).PreserveThreadContext();

                this.Logger.Debug("Notified itself that it is starting.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.ApplicationStartedEvent_Exception);
                opResult.MergeException(ex);
            }

            return opResult;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>A Task.</returns>
        public async Task<IOperationResult> AfterAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStartingEvent = this.CreateAppStartingEvent();
            var appStartedEvent = this.CreateAppStartedEvent();

            try
            {
                // first of all notify its own manager that it started...
                await this.eventHub.PublishAsync(appStartedEvent, this.appContext, cancellationToken).PreserveThreadContext();

                // ...then the peers that it is starting and, at the same time, that it is started.
                // Reason: during the bootstrap it may happen that the communication channels between the peers are not open.
                await this.messageBroker.PublishAsync(
                    appStartingEvent,
                    ctx => ctx.Impersonate(this.appContext),
                    cancellationToken).PreserveThreadContext();
                await this.messageBroker.PublishAsync(
                    appStartedEvent,
                    ctx => ctx.Impersonate(this.appContext),
                    cancellationToken).PreserveThreadContext();

                this.Logger.Debug("Notified itself and peers that it is started.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.ApplicationStartedEvent_Exception);
                opResult.MergeException(ex);
            }

            return opResult;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> BeforeAppFinalizeAsync(CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStoppingEvent = this.CreateAppStoppingEvent();
            var appStoppedEvent = this.CreateAppStoppedEvent();

            try
            {
                // first of all notify its own manager that it is stopping...
                await this.eventHub.PublishAsync(appStoppingEvent, this.appContext, cancellationToken).PreserveThreadContext();

                // ...then the peers that it is stopping and, at the same time, that it is stopped.
                // Reason: during the shutdown it may happen that the communication channels between the peers are closed.
                await this.messageBroker.PublishAsync(
                    appStoppingEvent,
                    ctx => ctx.Impersonate(this.appContext),
                    cancellationToken).PreserveThreadContext();
                await this.messageBroker.PublishAsync(
                    appStoppedEvent,
                    ctx => ctx.Impersonate(this.appContext),
                    cancellationToken).PreserveThreadContext();

                this.Logger.Debug("Notified itself and peers that it is stopping.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.ApplicationStoppedEvent_Exception);
                opResult.MergeException(ex);
            }

            return opResult;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task<IOperationResult> AfterAppFinalizeAsync(CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStoppedEvent = this.CreateAppStoppedEvent();

            try
            {
                // now notify its own manager that it is stopped.
                await this.eventHub.PublishAsync(appStoppedEvent, this.appContext, cancellationToken).PreserveThreadContext();

                this.Logger.Debug("Notified itself that it is stopped.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.ApplicationStoppedEvent_Exception);
                opResult.MergeException(ex);
            }

            return opResult;
        }

        private async Task<IMessage?> HandleAppStartedEventAsync(AppStartedEvent message, IMessagingContext context, CancellationToken cancellationToken)
        {
            await this.eventHub.PublishAsync(message, context, cancellationToken).PreserveThreadContext();
            return null;
        }

        private async Task<IMessage?> HandleAppStoppedEventAsync(AppStoppedEvent message, IMessagingContext context, CancellationToken cancellationToken)
        {
            await this.eventHub.PublishAsync(message, context, cancellationToken).PreserveThreadContext();
            return null;
        }

        private async Task<IMessage?> HandleAppStartingEventAsync(AppStartingEvent message, IMessagingContext context, CancellationToken cancellationToken)
        {
            await this.eventHub.PublishAsync(message, context, cancellationToken).PreserveThreadContext();
            return null;
        }

        private async Task<IMessage?> HandleAppStoppingEventAsync(AppStoppingEvent message, IMessagingContext context, CancellationToken cancellationToken)
        {
            await this.eventHub.PublishAsync(message, context, cancellationToken).PreserveThreadContext();
            return null;
        }

        private AppStartingEvent CreateAppStartingEvent()
        {
            return new AppStartingEvent
            {
                AppInfo = this.hostInfoProvider.GetRuntimeAppInfo(),
                Timestamp = DateTimeOffset.Now,
            };
        }

        private AppStartedEvent CreateAppStartedEvent()
        {
            return new AppStartedEvent
                       {
                           AppInfo = this.hostInfoProvider.GetRuntimeAppInfo(),
                           Timestamp = DateTimeOffset.Now,
                       };
        }

        private AppStoppingEvent CreateAppStoppingEvent()
        {
            return new AppStoppingEvent
            {
                AppInfo = this.hostInfoProvider.GetRuntimeAppInfo(),
                Timestamp = DateTimeOffset.Now,
            };
        }

        private AppStoppedEvent CreateAppStoppedEvent()
        {
            return new AppStoppedEvent
                       {
                           AppInfo = this.hostInfoProvider.GetRuntimeAppInfo(),
                           Timestamp = DateTimeOffset.Now,
                       };
        }
    }
}