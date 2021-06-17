// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScheduler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Runtime;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;
    using Kephas.Workflow;

    /// <summary>
    /// Interface for scheduler.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IScheduler : IAsyncInitializable, IAsyncFinalizable
    {
        /// <summary>
        /// Gets the scheduled jobs.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>A query over the scheduled jobs.</returns>
        IQueryable<IJobInfo> GetScheduledJobs(Action<ISchedulingContext>? options = null);

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>A query over the running jobs.</returns>
        IQueryable<IJobResult> GetRunningJobs(Action<ISchedulingContext>? options = null);

        /// <summary>
        /// Gets the completed jobs.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>A query over the completed jobs.</returns>
        IQueryable<IJobResult> GetCompletedJobs(Action<ISchedulingContext>? options = null);

        /// <summary>
        /// Enqueues a new job using a scheduled job or its ID.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the operation output.</returns>
        Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the operation output.</returns>
        Task<IOperationResult<IJobInfo?>> DisableScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the operation output.</returns>
        Task<IOperationResult<IJobInfo?>> EnableScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be canceled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IOperationResult<IJobInfo?>> CancelScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels the running job asynchronously.
        /// </summary>
        /// <param name="runningJob">The running job to be canceled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IOperationResult<IJobResult?>> CancelRunningJobAsync(
            object runningJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels the trigger asynchronously.
        /// </summary>
        /// <param name="trigger">The trigger to be canceled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IOperationResult<ITrigger?>> CancelTriggerAsync(
            object trigger,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IScheduler"/>.
    /// </summary>
    public static class SchedulerExtensions
    {
        /// <summary>
        /// Enqueues a new job and starts it asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="target">Optional. The target instance used by the job.</param>
        /// <param name="arguments">Optional. The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. The trigger to start the job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            IJobInfo scheduledJob,
            object? target = null,
            IDynamic? arguments = null,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, scheduledJob, null, target, arguments, options, trigger, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job and starts it asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="target">Optional. The target instance used by the job.</param>
        /// <param name="arguments">Optional. The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. The trigger to start the job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            object scheduledJobId,
            object? target = null,
            IDynamic? arguments = null,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, null, scheduledJobId, target, arguments, options, trigger, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="friendlyName">The friendly name of the job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. The trigger to start the job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            Func<object?> operation,
            string? friendlyName = null,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(
                scheduler,
                new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, operation, friendlyName),
                null,
                null,
                options,
                trigger,
                cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="friendlyName">The friendly name of the job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. The trigger to start the job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            Action operation,
            string? friendlyName = null,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(
                scheduler,
                new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, operation, friendlyName),
                null,
                null,
                options,
                trigger,
                cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        /// <param name="friendlyName">The friendly name of the job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. The trigger to start the job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            Func<CancellationToken, Task<object?>> asyncOperation,
            string? friendlyName = null,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(
                scheduler,
                new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, asyncOperation, friendlyName),
                null,
                null,
                options,
                trigger,
                cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. The trigger.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        private static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            IJobInfo scheduledJob,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, scheduledJob, null, null, null, options, trigger, cancellationToken);
        }

        /// <summary>
        /// Starts a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a
        /// <see cref="IJobInfo"/>.
        /// </para>
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="trigger">Optional. A trigger for the scheduled job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        private static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            this IScheduler scheduler,
            IJobInfo? scheduledJob,
            object? scheduledJobId,
            object? target,
            IDynamic? arguments,
            Action<IActivityContext>? options = null,
            ITrigger? trigger = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduler, nameof(scheduler));

            return scheduler.EnqueueAsync(
                scheduledJob ?? scheduledJobId,
                ctx => ctx
                    .ScheduledJob(scheduledJob)
                    .ScheduledJobId(scheduledJobId ?? scheduledJob?.Id)
                    .Trigger(trigger)
                    .Activity(target, arguments, options),
                cancellationToken);
        }
    }
}
