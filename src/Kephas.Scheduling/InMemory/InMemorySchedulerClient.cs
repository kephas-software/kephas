// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemorySchedulerClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory job scheduler client class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow;

    /// <summary>
    /// An in memory job scheduler client.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InMemorySchedulerClient : Loggable, ISchedulerClient
    {
        private readonly IEventHub eventHub;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySchedulerClient"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="logManager">The log manager.</param>
        public InMemorySchedulerClient(
            IEventHub eventHub,
            IContextFactory contextFactory,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.eventHub = eventHub;
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Cancels the running job asynchronously.
        /// </summary>
        /// <param name="runningJobId">The job identifier.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public async Task<IJobResult> CancelRunningJobAsync(object runningJobId, CancellationToken cancellationToken = default)
        {
            var enqueueEvent = new CancelRunningJobEvent
            {
                RunningJobId = runningJobId,
            };
            var result = await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken).PreserveThreadContext();
            result.ThrowIfHasErrors();

            return new JobResult { RunningJobId = runningJobId }
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }

        /// <summary>
        /// Cancels the trigger asynchronously.
        /// </summary>
        /// <param name="triggerId">The trigger identifier.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public async Task<IJobResult> CancelTriggerAsync(object triggerId, CancellationToken cancellationToken = default)
        {
            var enqueueEvent = new CancelTriggerEvent
            {
                TriggerId = triggerId,
            };
            var result = await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken).PreserveThreadContext();
            result.ThrowIfHasErrors();

            return new JobResult(triggerId)
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }
    }
}