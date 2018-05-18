// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultOrchestrationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default orchestration manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System;
    using System.Diagnostics;
    using System.Linq;
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
    /// The default orchestration manager.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultOrchestrationManager : IOrchestrationManager
    {
        /// <summary>
        /// The application manifest.
        /// </summary>
        private readonly IAppManifest appManifest;

        /// <summary>
        /// The application runtime.
        /// </summary>
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// The message broker.
        /// </summary>
        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOrchestrationManager"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageBroker">The message broker.</param>
        public DefaultOrchestrationManager(
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
        public ILogger<DefaultOrchestrationManager> Logger { get; set; }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.timer = new Timer(this.OnHeartbeat, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            // TODO do not notify here, because we are in the startup phase
            try
            {
                var appStartedMessage = this.CreateAppStartedMessage();
                await this.messageBroker.ProcessOneWayAsync(
                    appStartedMessage,
                    context,
                    cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                // TODO localization
                this.Logger.Error(ex, "Exception after initializing application behavior.");
            }
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">(Optional) An optional context for finalization.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            if (this.timer == null)
            {
                // not properly initialized, possibly abnormal program termination.
                return;
            }

            this.timer.Dispose();

            // TODO do not notify here, because we are in the finalizing phase
            var stoppedMessage = this.messageBroker.CreateBrokeredMessageBuilder<BrokeredMessage>()
                .WithContent(this.CreateAppStoppedMessage())
                .OneWay()
                .BrokeredMessage;

            try
            {
                await this.messageBroker.DispatchAsync(stoppedMessage, context, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Exception before finalizing application behavior.");
            }
        }

        /// <summary>
        /// Creates application started message.
        /// </summary>
        /// <returns>
        /// The new application started message.
        /// </returns>
        private AppStartedMessage CreateAppStartedMessage()
        {
            return new AppStartedMessage
            {
                AppInfo = new AppInfo
                {
                    AppId = this.appManifest.AppId,
                    AppInstanceId = this.appManifest.AppInstanceId,
                    ProcessId = Process.GetCurrentProcess().Id,
                    Features = this.appManifest.Features.Select(f => f.Name).ToArray(),
                    HostName = this.appRuntime.GetHostName(),
                    HostAddress = this.appRuntime.GetHostAddress().ToString(),
                }
            };
        }

        /// <summary>
        /// Creates application stopped message.
        /// </summary>
        /// <returns>
        /// The new application stopped message.
        /// </returns>
        private AppStoppedMessage CreateAppStoppedMessage()
        {
            return new AppStoppedMessage
            {
                AppInfo = new AppInfo
                {
                    AppId = this.appManifest.AppId,
                    AppInstanceId = this.appManifest.AppInstanceId
                }
            };
        }

        /// <summary>
        /// Executes the heartbeat action.
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnHeartbeat(object state)
        {
            // TODO
        }
    }
}