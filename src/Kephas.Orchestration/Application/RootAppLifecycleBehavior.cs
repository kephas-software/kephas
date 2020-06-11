// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Lifecycle behavior for the root microservice. It should be processed as the latest of
    /// all the microservices in the system.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class RootAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppRuntime appRuntime;
        private readonly IOrchestrationManager orchestrationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="orchestrationManager">The orchestration manager.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public RootAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IOrchestrationManager orchestrationManager,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.orchestrationManager = orchestrationManager;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override async Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            if (!this.appRuntime.IsRoot())
            {
                return;
            }

            var appRoles = (await this.orchestrationManager.GetLiveAppsAsync(cancellationToken: cancellationToken).PreserveThreadContext()).ToList();
            var startTasks = new List<Task<ProcessStartResult>>();
            this.Logger.Info("Starting worker application instances...");

            //... TODO
        }
    }
}