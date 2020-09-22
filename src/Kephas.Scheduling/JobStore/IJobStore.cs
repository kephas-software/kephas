// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.JobStore
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for working with the job store.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IJobStore
    {
        /// <summary>
        /// Gets the scheduled job based on its ID asynchronously.
        /// </summary>
        /// <param name="jobId">The scheduled job ID.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw if the job is not found.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result yielding the scheduled job.</returns>
        Task<IJobInfo?> GetScheduledJobAsync(object jobId, bool throwOnNotFound = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the scheduled job with the provided ID asynchronously.
        /// </summary>
        /// <param name="jobId">The scheduled job ID.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result yielding a value indicating whether the job was removed or not.</returns>
        Task<bool> RemoveScheduledJobAsync(object jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a scheduled job asynchronously.
        /// </summary>
        /// <param name="job">The job to add.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task AddScheduledJobAsync(IJobInfo job, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the result of a completed job asynchronously.
        /// </summary>
        /// <param name="completedJob">The completed job result.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task AddCompletedJobResultAsync(IJobResult completedJob, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the running job based on its ID asynchronously.
        /// </summary>
        /// <param name="runningJobId">The running job ID.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw if the job is not found.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result yielding the scheduled job.</returns>
        Task<IJobResult?> GetRunningJobAsync(object runningJobId, bool throwOnNotFound = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a running job asynchronously.
        /// </summary>
        /// <param name="runningJob">The running job result.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task AddRunningJobAsync(IJobResult runningJob, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a running job asynchronously.
        /// </summary>
        /// <param name="runningJobId">The running job identifier.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task RemoveRunningJobAsync(object runningJobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a trigger associated to the scheduled job asynchronously.
        /// </summary>
        /// <param name="trigger">The trigger to add.</param>
        /// <param name="scheduledJob">The existing scheduled job.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task AddTriggerAsync(ITrigger trigger, IJobInfo scheduledJob, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a trigger asynchronously.
        /// </summary>
        /// <param name="triggerId">The identifier of the trigger to remove.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task RemoveTriggerAsync(object triggerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the enabled flag of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="isEnabled">The enabled flag.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task SetTriggerEnabledAsync(ITrigger trigger, bool isEnabled, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the scheduled jobs.
        /// </summary>
        /// <returns>A query over the scheduled jobs.</returns>
        IQueryable<IJobInfo> GetScheduledJobs();

        /// <summary>
        /// Gets the completed jobs.
        /// </summary>
        /// <returns>A query over the completed jobs.</returns>
        IQueryable<IJobResult> GetCompletedJobs();

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <returns>A query over the running jobs.</returns>
        IQueryable<IJobResult> GetRunningJobs();
    }
}