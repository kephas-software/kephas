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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;

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
}
