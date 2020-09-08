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

        private readonly ConcurrentDictionary<object, (ITrigger trigger, IJobInfo scheduledJob, Func<CancellationToken, IJobResult> triggerAction)>
            activeTriggers = new ConcurrentDictionary<object, (ITrigger trigger, IJobInfo scheduledJob, Func<CancellationToken, IJobResult> triggerAction)>();

        private readonly ConcurrentDictionary<object, (IJobResult jobResult, CancellationTokenSource cancellationSource)>
            runningJobs = new ConcurrentDictionary<object, (IJobResult jobResult, CancellationTokenSource cancellationSource)>();

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
            this.subscriptions.Add(this.eventHub.Subscribe<CancelRunningJobEvent>(this.HandleCancelRunningJob));
            this.subscriptions.Add(this.eventHub.Subscribe<CancelScheduledJobEvent>(this.HandleCancelScheduledJob));
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

            // stop running jobs
            while (this.runningJobs.Count > 0)
            {
                var kv = this.runningJobs.FirstOrDefault();
                if (kv.Key != null)
                {
                    this.CancelRunningJob(kv.Key);
                }
            }

            // stop scheduled jobs
            lock (this.scheduledJobs)
            {
                while (this.scheduledJobs.Count > 0)
                {
                    this.CancelScheduledJob(this.scheduledJobs[0]);
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
            return this.runningJobs.Values
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
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        public Task<IOperationResult> DisableScheduledJobAsync(
            object scheduledJob,
            CancellationToken cancellationToken = default)
            => this.ToggleScheduledJobAsync(scheduledJob, false, cancellationToken);

        /// <summary>
        /// Enables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        public virtual Task<IOperationResult> EnableScheduledJobAsync(
            object scheduledJob,
            CancellationToken cancellationToken = default)
            => this.ToggleScheduledJobAsync(scheduledJob, true, cancellationToken);

        /// <summary>
        /// Depending on the <paramref name="enabled"/> argument, enables or disables all the triggers
        /// of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="enabled">Indicates whether to enable or to disable the scheduled job.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        protected virtual async Task<IOperationResult> ToggleScheduledJobAsync(
            object scheduledJob,
            bool enabled,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            void toggleTriggers(IJobInfo jobInfo)
            {
                jobInfo.Triggers.ForEach(t => t.IsEnabled = enabled);
            }

            if (scheduledJob is IJobInfo jobInfo)
            {
                toggleTriggers(jobInfo);
            }
            else
            {
                lock (this.scheduledJobs)
                {
                    jobInfo = this.scheduledJobs.FirstOrDefault(j => j.Id.Equals(scheduledJob));
                    if (jobInfo == null)
                    {
                        return new OperationResult().Fail(new KeyNotFoundException($"Scheduled job '{scheduledJob}' was not found."));
                    }

                    toggleTriggers(jobInfo);
                }
            }

            var enableString = enabled ? "enabled" : "disabled";
            return new OperationResult()
                .MergeMessage($"Scheduled job '{scheduledJob}' was {enableString}.")
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

            if (e.ScheduledJob == null)
            {
                throw new ArgumentException(
                    $"The {nameof(e.ScheduledJob)} parameter in the {nameof(EnqueueEvent)} is not set.",
                    nameof(e.ScheduledJob));
            }

            var jobInfo = e.ScheduledJob;
            var activityContext = this.CreateActivityContext(e.Options);

            var trigger = activityContext.Trigger()
                          ?? new TimerTrigger(e.TriggerId ?? Guid.NewGuid());

            if (!jobInfo.AddTrigger(trigger))
            {
                this.Logger.Warn("Could not add trigger '{trigger}' to scheduled job '{scheduledJob}'", trigger, jobInfo);
            }

            lock (this.scheduledJobs)
            {
                this.scheduledJobs.Add(jobInfo);
            }

            if (!this.activeTriggers.TryAdd(
                trigger.Id,
                (trigger, jobInfo, ct => this.StartJob(jobInfo, e.Target, e.Arguments, e.Options, ct))))
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
                if (!this.runningJobs.TryAdd(jobResult.RunningJobId!, (jobResult, cancellationSource)))
                {
                    this.Logger.Warn(
                        "Cannot add the job '{runningJobId}' to the list of running jobs.",
                        jobResult.RunningJobId);
                }

                // upon completion, remove the job from the running collection.
                var jobTask = jobResult.AsTask();
                jobTask.ContinueWith(
                    t =>
                    {
                        if (jobResult is JobResult jobResultClass)
                        {
                            jobResultClass.EndedAt = DateTimeOffset.Now;
                            jobResultClass.PercentCompleted = 1;
                        }

                        this.runningJobs.TryRemove(jobResult.RunningJobId!, out _);
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

            this.Logger.Info("Enqueued job '{scheduledJob}' with trigger '{trigger}'.", jobInfo, trigger);

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
                tuple.scheduledJob.RemoveTrigger(tuple.trigger);
                this.Logger.Info("Trigger with ID '{triggerId}' was removed from the list of active triggers and was canceled.", triggerId);
            }
        }

        /// <summary>
        /// Handles the cancel running job event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelRunningJob(CancelRunningJobEvent e, IContext context)
        {
            Requires.NotNull(e.RunningJobId, nameof(e.RunningJobId));

            this.CancelRunningJob(e.RunningJobId);
        }

        /// <summary>
        /// Cancels the running job with the provided ID.
        /// </summary>
        /// <param name="runningJobId">The running job ID.</param>
        protected virtual void CancelRunningJob(object runningJobId)
        {
            this.Logger.Info("Cancelling running job with ID '{runningJobId}'...", runningJobId);

            if (!this.runningJobs.TryRemove(runningJobId, out var tuple))
            {
                this.Logger.Warn(
                    "Job with ID '{runningJobId}' was already removed from the list of running jobs.",
                    runningJobId);
            }
            else
            {
                tuple.cancellationSource.Cancel();
                this.Logger.Info("Job with ID '{runningJobId}' was removed from the list of running jobs and was signaled for cancellation.", runningJobId);
            }
        }

        /// <summary>
        /// Handles the <see cref="CancelScheduledJobEvent"/>.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelScheduledJob(CancelScheduledJobEvent e, IContext context)
        {
            if (e.ScheduledJob == null && e.ScheduledJobId == null)
            {
                throw new ArgumentNullException(nameof(e.ScheduledJob),
                    $"Either the {nameof(e.ScheduledJob)} or {nameof(e.ScheduledJobId)} must be provided.");
            }

            this.CancelScheduledJob(e.ScheduledJob ?? e.ScheduledJobId);
        }

        /// <summary>
        /// Handles the <see cref="CancelScheduledJobEvent"/>.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job or its ID.</param>
        protected virtual void CancelScheduledJob(object? scheduledJob)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            this.Logger.Info("Cancelling jobs and triggers based on scheduled job '{scheduledJob}'...", scheduledJob);

            IJobInfo[] matchingScheduledJobs;
            lock (this.scheduledJobs)
            {
                matchingScheduledJobs = scheduledJob is IJobInfo jobInfo
                    ? new[] { jobInfo }
                    : this.scheduledJobs.Where(j => j.Equals(scheduledJob) || j.Id.Equals(scheduledJob)).ToArray();
                matchingScheduledJobs.ForEach(j => this.scheduledJobs.Remove(j));
            }

            if (matchingScheduledJobs.Length == 0)
            {
                this.Logger.Info("No scheduled jobs found for '{scheduledJob}'.", scheduledJob);
                return;
            }

            // first of all cancel all matching triggers...
            matchingScheduledJobs.ForEach(ji => ji.Triggers.ForEach(t => this.CancelTrigger(t.Id)));

            // ...then all matching running jobs.
            var matchingRunningJobs = this.runningJobs
                    .Where(kv =>
                        scheduledJob.Equals(kv.Value.jobResult.ScheduledJobId)
                        || scheduledJob.Equals(kv.Value.jobResult.ScheduledJob))
                    .ToList();
            if (matchingRunningJobs.Count == 0)
            {
                this.Logger.Info("No running jobs found for '{scheduledJob}'.", scheduledJob);
                return;
            }

            var matchingRunningJobIds = matchingRunningJobs
                .Select(kv => kv.Key)
                .ToList();
            matchingRunningJobIds.ForEach(this.CancelRunningJob);
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
        /// Starts the operation associated to the scheduled job.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="target">The target.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The running job result.</returns>
        protected virtual IJobResult StartJob(
            IJobInfo scheduledJob,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options,
            CancellationToken cancellationToken)
        {
            var job = (IJob) scheduledJob.CreateInstance();
            job.Target = target;
            job.Arguments = arguments;
            var startedAt = DateTimeOffset.Now;

            return new JobResult(
                job.Id,
                this.workflowProcessor.ExecuteAsync(job, target, arguments, options, cancellationToken))
            {
                ScheduledJob = scheduledJob,
                RunningJob = job,
                StartedAt = startedAt,
                OperationState = OperationState.InProgress,
            };
        }
    }
}