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
    using Kephas.Diagnostics.Contracts;
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
    public class OrchestrationAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppRuntime appRuntime;
        private readonly IMessageBroker messageBroker;
        private readonly IMessageHandlerRegistry messageHandlerRegistry;
        private readonly IEventHub eventHub;
        private readonly IHostInfoProvider hostInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageBroker">The application event publisher.</param>
        /// <param name="messageHandlerRegistry">The message handler registry.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="hostInfoProvider">The host information provider.</param>
        public OrchestrationAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IMessageBroker messageBroker,
            IMessageHandlerRegistry messageHandlerRegistry,
            IEventHub eventHub,
            IHostInfoProvider hostInfoProvider)
        {
            appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(messageHandlerRegistry, nameof(messageHandlerRegistry));
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            Requires.NotNull(hostInfoProvider, nameof(hostInfoProvider));

            this.appRuntime = appRuntime;
            this.messageBroker = messageBroker;
            this.messageHandlerRegistry = messageHandlerRegistry;
            this.eventHub = eventHub;
            this.hostInfoProvider = hostInfoProvider;
            this.messageHandlerRegistry.RegisterHandler<AppStoppedEvent>(this.HandleAppStoppedEventAsync);
            this.messageHandlerRegistry.RegisterHandler<AppStoppingEvent>(this.HandleAppStoppingEventAsync);
            this.messageHandlerRegistry.RegisterHandler<AppStartedEvent>(this.HandleAppStartedEventAsync);
            this.messageHandlerRegistry.RegisterHandler<AppStartingEvent>(this.HandleAppStartingEventAsync);
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
        public override async Task<IOperationResult> BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStartingEvent = this.CreateAppStartingEvent();

            try
            {
                // notify its own manager that it is starting.
                await this.eventHub.PublishAsync(appStartingEvent, appContext, cancellationToken).PreserveThreadContext();

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
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>A Task.</returns>
        public override async Task<IOperationResult> AfterAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStartingEvent = this.CreateAppStartingEvent();
            var appStartedEvent = this.CreateAppStartedEvent();

            try
            {
                // first of all notify its own manager that it started...
                await this.eventHub.PublishAsync(appStartedEvent, appContext, cancellationToken).PreserveThreadContext();

                // ...then the peers that it is starting and, at the same time, that it is started.
                // Reason: during the bootstrap it may happen that the communication channels between the peers are not open.
                await this.messageBroker.PublishAsync(
                    appStartingEvent,
                    ctx => ctx.Impersonate(appContext),
                    cancellationToken).PreserveThreadContext();
                await this.messageBroker.PublishAsync(
                    appStartedEvent,
                    ctx => ctx.Impersonate(appContext),
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
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override async Task<IOperationResult> BeforeAppFinalizeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStoppingEvent = this.CreateAppStoppingEvent();
            var appStoppedEvent = this.CreateAppStoppedEvent();

            try
            {
                // first of all notify its own manager that it is stopping...
                await this.eventHub.PublishAsync(appStoppingEvent, appContext, cancellationToken).PreserveThreadContext();

                // ...then the peers that it is stopping and, at the same time, that it is stopped.
                // Reason: during the shutdown it may happen that the communication channels between the peers are closed.
                await this.messageBroker.PublishAsync(
                    appStoppingEvent,
                    ctx => ctx.Impersonate(appContext),
                    cancellationToken).PreserveThreadContext();
                await this.messageBroker.PublishAsync(
                    appStoppedEvent,
                    ctx => ctx.Impersonate(appContext),
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
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override async Task<IOperationResult> AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            var opResult = true.ToOperationResult();
            var appStoppedEvent = this.CreateAppStoppedEvent();

            try
            {
                // now notify its own manager that it is stopped.
                await this.eventHub.PublishAsync(appStoppedEvent, appContext, cancellationToken).PreserveThreadContext();

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