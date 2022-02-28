// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryJobStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.JobStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Linq;
    using Kephas.Logging;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;

    /// <summary>
    /// Provides the in-memory implementation of the <see cref="IJobStore"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InMemoryJobStore : Loggable, IJobStore
    {
        private readonly ConcurrentQueue<IJobResult> completedJobs = new ();

        private readonly ConcurrentDictionary<object, IJobInfo> scheduledJobs = new ();

        private readonly ConcurrentDictionary<object, IJobResult> runningJobs = new ();

        private readonly ConcurrentDictionary<object, (ITrigger trigger, IJobInfo scheduledJob)> activeTriggers = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryJobStore"/> class.
        /// </summary>
        /// <param name="logManager">Optional.The log manager.</param>
        public InMemoryJobStore(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Gets the scheduled job based on its ID.
        /// </summary>
        /// <param name="jobId">The scheduled job ID.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw if the job is not found.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result yielding the scheduled job.</returns>
        public Task<IJobInfo?> GetScheduledJobAsync(object jobId, bool throwOnNotFound = true, CancellationToken cancellationToken = default)
        {
            jobId = jobId ?? throw new ArgumentNullException(nameof(jobId));

            if (!this.scheduledJobs.TryGetValue(jobId, out var jobInfo) && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Scheduled job '{jobId}' not found.");
            }

            return Task.FromResult<IJobInfo?>(jobInfo);
        }

        /// <summary>
        /// Removes the scheduled job with the provided ID.
        /// </summary>
        /// <param name="jobId">The job ID.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result yielding a value indicating whether the job was removed or not.</returns>
        public Task<bool> RemoveScheduledJobAsync(object jobId, CancellationToken cancellationToken = default)
        {
            jobId = jobId ?? throw new ArgumentNullException(nameof(jobId));

            return Task.FromResult(this.scheduledJobs.TryRemove(jobId, out _));
        }

        /// <summary>
        /// Adds a scheduled job asynchronously and returns a value indicating whether the job was added.
        /// A job cannot be added if it is already in the store.
        /// </summary>
        /// <param name="job">The job to add.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding a value indicating whether the job was added.</returns>
        public Task<bool> AddScheduledJobAsync(IJobInfo job, CancellationToken cancellationToken = default)
        {
            job = job ?? throw new ArgumentNullException(nameof(job));

            if (!this.scheduledJobs.TryAdd(job.Id, job))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Adds the result of a completed job asynchronously and returns a value indicating whether the job was added.
        /// A completed job cannot be added if it is already in the store.
        /// </summary>
        /// <param name="completedJob">The completed job result.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding a value indicating whether the completed job was added.</returns>
        public Task<bool> AddCompletedJobResultAsync(
            IJobResult completedJob,
            CancellationToken cancellationToken = default)
        {
            completedJob = completedJob ?? throw new ArgumentNullException(nameof(completedJob));

            this.completedJobs.Enqueue(completedJob);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the running job based on its ID asynchronously.
        /// </summary>
        /// <param name="runningJobId">The running job ID.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw if the job is not found.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result yielding the scheduled job.</returns>
        public Task<IJobResult?> GetRunningJobAsync(object runningJobId, bool throwOnNotFound = true, CancellationToken cancellationToken = default)
        {
            runningJobId = runningJobId ?? throw new ArgumentNullException(nameof(runningJobId));

            if (!this.runningJobs.TryGetValue(runningJobId, out var jobResult) && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Running job '{runningJobId}' not found.");
            }

            return Task.FromResult<IJobResult?>(jobResult);
        }

        /// <summary>
        /// Adds a running job asynchronously.
        /// </summary>
        /// <param name="runningJob">The running job result.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public Task AddRunningJobAsync(IJobResult runningJob, CancellationToken cancellationToken = default)
        {
            runningJob = runningJob ?? throw new ArgumentNullException(nameof(runningJob));

            if (!this.runningJobs.TryAdd(runningJob.RunningJobId!, runningJob))
            {
                throw new InvalidOperationException($"Could not add the running job '{runningJob}'.");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a running job asynchronously.
        /// </summary>
        /// <param name="runningJobId">The running job identifier.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public Task RemoveRunningJobAsync(object runningJobId, CancellationToken cancellationToken = default)
        {
            runningJobId = runningJobId ?? throw new ArgumentNullException(nameof(runningJobId));

            return Task.FromResult(this.runningJobs.TryRemove(runningJobId, out _));
        }

        /// <summary>
        /// Adds a trigger associated to the scheduled job.
        /// </summary>
        /// <param name="trigger">The trigger to add.</param>
        /// <param name="scheduledJob">The existing scheduled job.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public Task AddTriggerAsync(ITrigger trigger, IJobInfo scheduledJob, CancellationToken cancellationToken = default)
        {
            trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            scheduledJob = scheduledJob ?? throw new ArgumentNullException(nameof(scheduledJob));

            if (!this.activeTriggers.TryAdd(trigger.Id, (trigger, scheduledJob)))
            {
                this.Logger.Warn("Cannot add trigger with ID {triggerId}.", trigger.Id);
            }

            if (!scheduledJob.AddTrigger(trigger))
            {
                this.Logger.Warn("Could not add trigger '{trigger}' to scheduled job '{scheduledJob}'", trigger, scheduledJob);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a trigger asynchronously.
        /// </summary>
        /// <param name="triggerId">The identifier of the trigger to remove.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public Task RemoveTriggerAsync(object triggerId, CancellationToken cancellationToken = default)
        {
            triggerId = triggerId ?? throw new ArgumentNullException(nameof(triggerId));

            if (!this.activeTriggers.TryRemove(triggerId, out var tuple))
            {
                this.Logger.Warn("Could not remove trigger with ID '{triggerId}'", triggerId);
                return Task.CompletedTask;
            }

            if (!tuple.scheduledJob.RemoveTrigger(tuple.trigger))
            {
                this.Logger.Warn("Could not remove trigger '{trigger}' from scheduled job '{scheduledJob}'", tuple.trigger, tuple.scheduledJob);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the enabled flag of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="isEnabled">The enabled flag.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public Task SetTriggerEnabledAsync(ITrigger trigger, bool isEnabled, CancellationToken cancellationToken = default)
        {
            trigger.IsEnabled = isEnabled;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the scheduled jobs.
        /// </summary>
        /// <returns>A query over the scheduled jobs.</returns>
        public IDisposableQueryable<IJobInfo> GetScheduledJobs()
        {
            return new DisposableQueryable<IJobInfo>(this.scheduledJobs.Values.ToArray().AsQueryable());
        }

        /// <summary>
        /// Gets the completed jobs.
        /// </summary>
        /// <returns>A query over the completed jobs.</returns>
        public IDisposableQueryable<IJobResult> GetCompletedJobs()
        {
            return new DisposableQueryable<IJobResult>(this.completedJobs.ToArray().AsQueryable());
        }

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <returns>A query over the running jobs.</returns>
        public IDisposableQueryable<IJobResult> GetRunningJobs()
        {
            return new DisposableQueryable<IJobResult>(this.runningJobs.Values.ToArray().AsQueryable());
        }
    }
}