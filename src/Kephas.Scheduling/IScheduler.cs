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
        /// Disables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="job">The ID of the job or the job to be disabled.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        Task<IOperationResult> DisableScheduledJobAsync(object job, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="job">The ID of the job or the job to be enabled.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        Task<IOperationResult> EnableScheduledJobAsync(object job, CancellationToken cancellationToken = default);
    }
}
