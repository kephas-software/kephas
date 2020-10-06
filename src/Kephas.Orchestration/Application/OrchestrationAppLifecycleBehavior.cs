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
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageBroker">The application event publisher.</param>
        /// <param name="eventHub">The event hub.</param>
        public OrchestrationAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IMessageBroker messageBroker,
            IEventHub eventHub)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(eventHub, nameof(eventHub));

            this.appRuntime = appRuntime;
            this.messageBroker = messageBroker;
            this.eventHub = eventHub;
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
            try
            {
                var appStartedEvent = this.CreateAppStartedEvent();

                // first of all notify its own manager that it started...
                await this.eventHub.PublishAsync(appStartedEvent, appContext, cancellationToken).PreserveThreadContext();

                // ...then the peers.
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
            var appStoppedEvent = this.CreateAppStoppedEvent();
            try
            {
                await this.messageBroker.PublishAsync(appStoppedEvent, ctx => ctx.Impersonate(appContext), cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.ApplicationStoppedEvent_Exception);
                opResult.MergeException(ex);
            }

            return opResult;
        }

        private AppStartedEvent CreateAppStartedEvent()
        {
            return new AppStartedEvent
                       {
                           AppInfo = this.appRuntime.GetRuntimeAppInfo(),
                           Timestamp = DateTimeOffset.Now,
                       };
        }

        private AppStoppedEvent CreateAppStoppedEvent()
        {
            return new AppStoppedEvent
                       {
                           AppInfo = this.appRuntime.GetRuntimeAppInfo(),
                           Timestamp = DateTimeOffset.Now,
                       };
        }
    }
}