// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationAppLifecycleBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An orchestration application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class OrchestrationAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppManifest appManifest;

        private readonly IAppRuntime appRuntime;

        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageBroker">The message broker.</param>
        public OrchestrationAppLifecycleBehavior(
            IAppManifest appManifest,
            IAppRuntime appRuntime,
            IMessageBroker messageBroker)
        {
            Requires.NotNull(appManifest, nameof(appManifest));
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(messageBroker, nameof(messageBroker));

            this.appManifest = appManifest;
            this.appRuntime = appRuntime;
            this.messageBroker = messageBroker;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<OrchestrationAppLifecycleBehavior> Logger { get; set; }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>A Task.</returns>
        public override async Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            try
            {
                var appStartedEvent = this.CreateAppStartedEvent();
                await this.messageBroker.PublishAsync(
                    appStartedEvent,
                    appContext,
                    cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                // TODO localization
                this.Logger.Error(ex, "Exception when publishing the application started event.");
            }
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
        public override async Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            var appStoppedEvent = this.CreateAppStoppedEvent();
            try
            {
                await this.messageBroker.PublishAsync(appStoppedEvent, appContext, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Exception when publishing the application stopped event.");
            }
        }

        private AppStartedEvent CreateAppStartedEvent()
        {
            return new AppStartedEvent
                       {
                           AppInfo = this.appManifest.GetAppInfo(this.appRuntime),
                           Timestamp = DateTimeOffset.Now
                       };
        }

        private AppStoppedEvent CreateAppStoppedEvent()
        {
            return new AppStoppedEvent
                       {
                           AppInfo = this.appManifest.GetAppInfo(this.appRuntime),
                           Timestamp = DateTimeOffset.Now
                       };
        }
    }
}