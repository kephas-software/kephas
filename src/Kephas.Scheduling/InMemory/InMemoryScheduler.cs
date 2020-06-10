// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryScheduler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory scheduler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Workflow;

    /// <summary>
    /// An in memory scheduler.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InMemoryScheduler : Loggable, IScheduler
    {
        private readonly IEventHub eventHub;
        private readonly IContextFactory contextFactory;
        private readonly IWorkflowProcessor workflowProcessor;
        private readonly List<IEventSubscription> subscriptions = new List<IEventSubscription>();

        private readonly
            ConcurrentDictionary<object, (ITrigger trigger, Func<CancellationToken, IJobResult> triggerAction)>
            activeTriggers
                = new ConcurrentDictionary<object, (ITrigger trigger, Func<CancellationToken, IJobResult> triggerAction)
                >();

        private readonly
            ConcurrentDictionary<object, (IJobResult jobResult, CancellationTokenSource cancellationSource)> activeJobs
                = new ConcurrentDictionary<object, (IJobResult jobResult, CancellationTokenSource cancellationSource)
                >();

        private readonly ConcurrentQueue<IJobResult> completedJobs = new ConcurrentQueue<IJobResult>();

        private readonly List<IJobInfo> scheduledJobs = new List<IJobInfo>();

        private readonly FinalizationMonitor<IScheduler> finalizationMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryScheduler"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="workflowProcessor">The workflow processor.</param>
        /// <param name="logManager">The log manager.</param>
        public InMemoryScheduler(
            IEventHub eventHub,
            IContextFactory contextFactory,
            IWorkflowProcessor workflowProcessor,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.eventHub = eventHub;
            this.contextFactory = contextFactory;
            this.workflowProcessor = workflowProcessor;
            this.finalizationMonitor = new FinalizationMonitor<IScheduler>(this.GetType());
        }

        /// <summary>
        /// Initializes the scheduler asynchronously.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task InitializeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.subscriptions.Add(this.eventHub.Subscribe<EnqueueEvent>(this.HandleEnqueue));
            this.subscriptions.Add(this.eventHub.Subscribe<CancelJobEvent>(this.HandleCancelJob));
            this.subscriptions.Add(this.eventHub.Subscribe<CancelJobInfoEvent>(this.HandleCancelJobInfo));
            this.subscriptions.Add(this.eventHub.Subscribe<CancelTriggerEvent>(this.HandleCancelTrigger));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Finalizes the scheduler asynchronously.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task FinalizeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.finalizationMonitor.Start();

            this.subscriptions.ForEach(s => s.Dispose());

            // stop triggers
            while (this.activeTriggers.Count > 0)
            {
                var kv = this.activeTriggers.FirstOrDefault();
                if (kv.Key != null)
                {
                    this.CancelTrigger(kv.Key);
                }
            }

            // stop jobs
            while (this.activeJobs.Count > 0)
            {
                var kv = this.activeJobs.FirstOrDefault();
                if (kv.Key != null)
                {
                    this.CancelJob(kv.Key);
                }
            }

            this.finalizationMonitor.Complete();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the scheduled jobs.
        /// </summary>
        /// <returns>A query over the scheduled jobs.</returns>
        public IQueryable<IJobInfo> GetScheduledJobs()
        {
            lock (this.scheduledJobs)
            {
                return this.scheduledJobs.ToArray().AsQueryable();
            }
        }

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <returns>A query over the running jobs.</returns>
        public IQueryable<IJobResult> GetRunningJobs()
        {
            return this.activeJobs.Values
                .Select(j => j.jobResult)
                .ToList()
                .AsQueryable();
        }

        /// <summary>
        /// Gets the completed jobs.
        /// </summary>
        /// <returns>A query over the completed jobs.</returns>
        public IQueryable<IJobResult> GetCompletedJobs()
        {
            return this.completedJobs.ToList().AsQueryable();
        }

        /// <summary>
        /// Disables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="job">The ID of the job or the job to be disabled.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        public async Task<IOperationResult> DisableScheduledJobAsync(object job, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(job, nameof(job));

            await Task.Yield();

            if (job is IJobInfo jobInfo)
            {
                jobInfo.Triggers.ForEach(t => t.IsEnabled = false);
            }
            else
            {
                lock (this.scheduledJobs)
                {
                    jobInfo = this.scheduledJobs.FirstOrDefault(j => j.Id.Equals(job));
                    if (jobInfo == null)
                    {
                        return new OperationResult().Fail(new KeyNotFoundException($"Job with ID '{job}' was not found."));
                    }

                    jobInfo.Triggers.ForEach(t => t.IsEnabled = false);
                }
            }

            return new OperationResult()
                .MergeMessage($"Job with ID '{job}' was disabled.")
                .Complete();
        }

        /// <summary>
        /// Enables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="job">The ID of the job or the job to be enabled.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        public async Task<IOperationResult> EnableScheduledJobAsync(object job, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(job, nameof(job));

            await Task.Yield();

            if (job is IJobInfo jobInfo)
            {
                jobInfo.Triggers.ForEach(t => t.IsEnabled = true);
            }
            else
            {
                lock (this.scheduledJobs)
                {
                    jobInfo = this.scheduledJobs.FirstOrDefault(j => j.Id.Equals(job));
                    if (jobInfo == null)
                    {
                        return new OperationResult().Fail(new KeyNotFoundException($"Job with ID '{job}' was not found."));
                    }

                    jobInfo.Triggers.ForEach(t => t.IsEnabled = true);
                }
            }

            return new OperationResult()
                .MergeMessage($"Job with ID '{job}' was enabled.")
                .Complete();
        }

        /// <summary>
        /// Handles the enqueue event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleEnqueue(EnqueueEvent e, IContext context)
        {
            if (!this.finalizationMonitor.IsNotStarted)
            {
                this.Logger.Warn("Finalization is started, enqueue requests are rejected.");
                return;
            }

            if (e.JobInfo == null)
            {
                throw new ArgumentException(
                    $"The {nameof(e.JobInfo)} parameter in the {nameof(EnqueueEvent)} is not set.",
                    nameof(e.JobInfo));
            }

            var jobInfo = e.JobInfo;
            var activityContext = this.CreateActivityContext(e.Options);

            var trigger = activityContext.Trigger()
                          ?? new TimerTrigger(e.TriggerId ?? Guid.NewGuid());

            if (!jobInfo.AddTrigger(trigger))
            {
                this.Logger.Warn("Could not add trigger '{trigger}' to job '{job}'", trigger, jobInfo);
            }

            lock (this.scheduledJobs)
            {
                this.scheduledJobs.Add(jobInfo);
            }

            if (!this.activeTriggers.TryAdd(
                trigger.Id,
                (trigger, ct => this.StartJob(jobInfo, e.Target, e.Arguments, e.Options, ct))))
            {
                this.Logger.Warn("Cannot enqueue trigger with ID {triggerId}.", trigger.Id);
                return;
            }

            void OnTriggerOnFire(object sender, FireEventArgs args)
            {
                var triggerId = trigger.Id;
                if (!this.activeTriggers.TryGetValue(triggerId, out var tuple))
                {
                    this.Logger.Warn("Trigger with ID {triggerId} not found.", triggerId);
                    return;
                }

                var cancellationSource = new CancellationTokenSource();
                var jobResult = tuple.triggerAction(cancellationSource.Token);
                if (!this.activeJobs.TryAdd(jobResult.JobId!, (jobResult, cancellationSource)))
                {
                    this.Logger.Warn(
                        "Cannot add the job with ID '{jobId}' to the list of active jobs.",
                        jobResult.JobId);
                }

                // upon completion, remove the job from the active collection.
                var jobTask = jobResult.AsTask();
                jobTask.ContinueWith(
                    t =>
                    {
                        if (jobResult is JobResult jobResultClass)
                        {
                            jobResultClass.EndedAt = DateTimeOffset.Now;
                            jobResultClass.PercentCompleted = 1;
                        }

                        this.activeJobs.TryRemove(jobResult.JobId!, out _);
                        this.completedJobs.Enqueue(jobResult);
                    });

                // also, upon completion, remove the job from the active collection.
                if (args.CompleteCallback != null)
                {
                    jobTask.ContinueWith(t => args.CompleteCallback(jobResult), cancellationSource.Token);
                }
            }

            void OnTriggerOnDisposed(object sender, EventArgs args)
            {
                var triggerId = trigger.Id;
                if (this.activeTriggers.TryRemove(triggerId, out _))
                {
                    trigger.Fire -= OnTriggerOnFire;
                    trigger.Disposed -= OnTriggerOnDisposed;
                    jobInfo.RemoveTrigger(trigger);
                    if (!jobInfo.Triggers.Any())
                    {
                        lock (this.scheduledJobs)
                        {
                            this.scheduledJobs.Remove(jobInfo);
                        }
                    }
                }
            }

            trigger.Fire += OnTriggerOnFire;
            trigger.Disposed += OnTriggerOnDisposed;

            this.Logger.Info("Enqueued job '{job}' with trigger '{trigger}'.", jobInfo, trigger);

            ServiceHelper.Initialize(trigger);
        }

        /// <summary>
        /// Handles the cancel trigger event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelTrigger(CancelTriggerEvent e, IContext context)
        {
            Requires.NotNull(e.TriggerId, nameof(e.TriggerId));

            this.CancelTrigger(e.TriggerId);
        }

        /// <summary>
        /// Cancels the trigger with the provided ID.
        /// </summary>
        /// <param name="triggerId">The trigger ID.</param>
        protected virtual void CancelTrigger(object triggerId)
        {
            Requires.NotNull(triggerId, nameof(triggerId));

            this.Logger.Info("Cancelling trigger with ID '{triggerId}'...", triggerId);

            if (!this.activeTriggers.TryRemove(triggerId, out var tuple))
            {
                this.Logger.Warn("Trigger with ID '{triggerId}' was already removed from the list of active triggers.",
                    triggerId);
            }
            else
            {
                tuple.trigger.IsEnabled = false;
                tuple.trigger.Dispose();
                this.Logger.Info("Trigger with ID '{triggerId}' is canceled.", triggerId);
            }
        }

        /// <summary>
        /// Handles the cancel job event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelJob(CancelJobEvent e, IContext context)
        {
            Requires.NotNull(e.JobId, nameof(e.JobId));

            this.CancelJob(e.JobId);
        }

        /// <summary>
        /// Cancels the job with the provided ID.
        /// </summary>
        /// <param name="jobId">The job ID.</param>
        protected virtual void CancelJob(object jobId)
        {
            this.Logger.Info("Cancelling job with ID '{jobId}'...", jobId);

            if (!this.activeJobs.TryRemove(jobId, out var tuple))
            {
                this.Logger.Warn(
                    "Job with ID '{jobId}' was already removed from the list of active jobs.",
                    jobId);
            }
            else
            {
                tuple.cancellationSource.Cancel();
                this.Logger.Info("Job with ID '{jobId}' was signaled for cancellation.", jobId);
            }
        }

        /// <summary>
        /// Handles the <see cref="CancelJobInfoEvent"/>.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelJobInfo(CancelJobInfoEvent e, IContext context)
        {
            if (e.JobInfo == null && e.JobInfoId == null)
            {
                throw new ArgumentNullException(nameof(e.JobInfo),
                    $"Either the {nameof(e.JobInfo)} or {nameof(e.JobInfoId)} must be provided.");
            }

            this.Logger.Info("Cancelling jobs and triggers based on job information '{jobInfo}'...",
                e.JobInfo ?? e.JobInfoId);

            e.JobInfo?.Triggers.ForEach(t => this.CancelTrigger(t.Id));

            var matchingActiveJobs = (e.JobInfo == null
                    ? this.activeJobs.Where(kv => e.JobInfoId.Equals(kv.Value.jobResult.JobInfoId))
                    : this.activeJobs.Where(kv => e.JobInfo.Equals(kv.Value.jobResult.JobInfo)))
                .Select(kv => kv.Key)
                .ToList();

            matchingActiveJobs.ForEach(this.CancelJob);
        }

        /// <summary>
        /// Creates a new activity context.
        /// </summary>
        /// <param name="options">The context options.</param>
        /// <returns>The new activity context.</returns>
        protected virtual IActivityContext CreateActivityContext(Action<IActivityContext>? options)
        {
            return this.contextFactory.CreateContext<ActivityContext>().Merge(options);
        }

        /// <summary>
        /// Starts the operation associated to the job.
        /// </summary>
        /// <param name="jobInfo">The job information.</param>
        /// <param name="target">The target.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The job result.</returns>
        protected virtual IJobResult StartJob(
            IJobInfo jobInfo,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options,
            CancellationToken cancellationToken)
        {
            var job = (IJob) jobInfo.CreateInstance();
            job.Target = target;
            job.Arguments = arguments;
            var startedAt = DateTimeOffset.Now;

            return new JobResult(
                job.Id,
                this.workflowProcessor.ExecuteAsync(job, target, arguments, options, cancellationToken))
            {
                JobInfo = jobInfo,
                Job = job,
                StartedAt = startedAt,
                OperationState = OperationState.InProgress,
            };
        }
    }
}