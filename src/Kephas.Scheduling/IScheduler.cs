// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScheduler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Scheduling
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
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
        /// <returns>A query over the scheduled jobs.</returns>
        IQueryable<IJobInfo> GetScheduledJobs();

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <returns>A query over the running jobs.</returns>
        IQueryable<IJobResult> GetRunningJobs();

        /// <summary>
        /// Gets the completed jobs.
        /// </summary>
        /// <returns>A query over the completed jobs.</returns>
        IQueryable<IJobResult> GetCompletedJobs();

        /// <summary>
        /// Enqueues a new job using a scheduled job or its ID.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding the operation output.</returns>
        Task<IOperationResult> EnqueueAsync(
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
        Task<IOperationResult> DisableScheduledJobAsync(
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
        Task<IOperationResult> EnableScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default);
    }
}
