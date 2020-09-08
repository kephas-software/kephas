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
    using Kephas.Services;
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
        /// Enqueues a new job and starts it asynchronously.
        /// </summary>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="target">The target instance used by the job.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public Task<IJobResult> EnqueueAsync(
            object scheduledJobId,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJobId, nameof(scheduledJobId));

            return this.EnqueueAsync(null, scheduledJobId, target, arguments, options, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job and starts it asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="target">The target instance used by the job.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public Task<IJobResult> EnqueueAsync(
            IJobInfo scheduledJob,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            return this.EnqueueAsync(scheduledJob, null, target, arguments, options, cancellationToken);
        }

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public Task<IJobResult> CancelScheduledJobAsync(object scheduledJobId, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJobId, nameof(scheduledJobId));

            return this.CancelScheduledJobAsync(null, scheduledJobId, cancellationToken);
        }

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public Task<IJobResult> CancelScheduledJobAsync(IJobInfo scheduledJob, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            return this.CancelScheduledJobAsync(scheduledJob, null, cancellationToken);
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
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

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
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

            return new JobResult(triggerId)
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        protected virtual async Task<IJobResult> CancelScheduledJobAsync(IJobInfo? scheduledJob, object? scheduledJobId, CancellationToken cancellationToken = default)
        {
            var enqueueEvent = new CancelScheduledJobEvent
            {
                ScheduledJob = scheduledJob,
                ScheduledJobId = scheduledJobId ?? scheduledJob?.Id,
            };
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

            return new JobResult { ScheduledJob = scheduledJob, ScheduledJobId = scheduledJobId ?? scheduledJob?.Id }
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }

        /// <summary>
        /// Starts a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a
        /// <see cref="IJobInfo"/>.
        /// </para>
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        protected virtual async Task<IJobResult> EnqueueAsync(
            IJobInfo? scheduledJob,
            object? scheduledJobId,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            var enqueueEvent = new EnqueueEvent
            {
                ScheduledJob = scheduledJob,
                ScheduledJobId = scheduledJobId ?? scheduledJob?.Id,
                TriggerId = Guid.NewGuid(),
                Target = target,
                Arguments = arguments,
                Options = options,
            };
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

            return new JobResult(enqueueEvent.TriggerId) { ScheduledJob = scheduledJob, ScheduledJobId = scheduledJobId ?? scheduledJob?.Id }
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }
    }
}