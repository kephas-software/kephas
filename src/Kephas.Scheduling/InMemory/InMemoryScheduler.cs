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

    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
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
            ConcurrentDictionary<object, (ITrigger trigger, Func<CancellationToken, IJobResult> triggerAction)> activeTriggers
                = new ConcurrentDictionary<object, (ITrigger trigger, Func<CancellationToken, IJobResult> triggerAction)>();

        private readonly
            ConcurrentDictionary<object, (IJobResult jobResult, CancellationTokenSource cancellationSource)> activeJobs
                = new ConcurrentDictionary<object, (IJobResult jobResult, CancellationTokenSource cancellationSource)>();

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
                    this.HandleCancelTrigger(new CancelTriggerEvent { TriggerId = kv.Key }, context!);
                }
            }

            // stop jobs
            while (this.activeJobs.Count > 0)
            {
                var kv = this.activeJobs.FirstOrDefault();
                if (kv.Key != null)
                {
                    this.HandleCancelJob(new CancelJobEvent { JobId = kv.Key }, context!);
                }
            }

            this.finalizationMonitor.Complete();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the scheduled jobs.
        /// </summary>
        /// <returns>An enumeration of scheduled jobs.</returns>
        public IEnumerable<IJobInfo> GetScheduledJobs()
        {
            lock (this.scheduledJobs)
            {
                return this.scheduledJobs.ToArray();
            }
        }

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <returns>An enumeration of running jobs.</returns>
        public IEnumerable<IJobResult> GetRunningJobs()
        {
            return this.activeJobs.Values
                .Select(j => j.jobResult)
                .ToList();
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

            if (!(e.JobInfo is IJobInfo jobInfoObject))
            {
                throw new ArgumentException(
                    $"Only {nameof(IJobInfo)} values accepted for the {nameof(e.JobInfo)} parameter.",
                    nameof(e.JobInfo));
            }

            var activityContext = this.CreateActivityContext(e.Options);

            var trigger = activityContext.Trigger()
                          ?? new TimerTrigger(e.TriggerId ?? Guid.NewGuid());

            if (!jobInfoObject.AddTrigger(trigger))
            {
                this.Logger.Warn("Could not add trigger '{trigger}' to job '{job}'", trigger, jobInfoObject);
            }

            lock (this.scheduledJobs)
            {
                this.scheduledJobs.Add(jobInfoObject);
            }

            if (!this.activeTriggers.TryAdd(
                trigger.Id,
                (trigger, ct => this.StartJob(jobInfoObject, e.Target, e.Arguments, e.Options, ct))))
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

                if (args.CompleteCallback != null)
                {
                    jobResult.AsTask().ContinueWith(t => args.CompleteCallback(jobResult), cancellationSource.Token);
                }
            }

            void OnTriggerOnDisposed(object sender, EventArgs args)
            {
                var triggerId = trigger.Id;
                if (this.activeTriggers.TryRemove(triggerId, out _))
                {
                    trigger.Fire -= OnTriggerOnFire;
                    trigger.Disposed -= OnTriggerOnDisposed;
                    jobInfoObject.RemoveTrigger(trigger);
                    if (!jobInfoObject.Triggers.Any())
                    {
                        lock (this.scheduledJobs)
                        {
                            this.scheduledJobs.Remove(jobInfoObject);
                        }
                    }
                }
            }

            trigger.Fire += OnTriggerOnFire;
            trigger.Disposed += OnTriggerOnDisposed;

            this.Logger.Info("Enqueued job '{job}' with trigger '{trigger}'.", jobInfoObject, trigger);

            ServiceHelper.Initialize(trigger);
        }

        /// <summary>
        /// Handles the cancel trigger event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelTrigger(CancelTriggerEvent e, IContext context)
        {
            this.Logger.Info("Cancelling trigger with ID '{triggerId}'...", e.TriggerId);

            if (!this.activeTriggers.TryRemove(e.TriggerId, out var tuple))
            {
                this.Logger.Warn("Trigger with ID '{triggerId}' was already removed from the list of active triggers.", e.TriggerId);
            }

            tuple.trigger.Dispose();

            this.Logger.Info("Trigger with ID '{triggerId}' is canceled.", e.TriggerId);
        }

        /// <summary>
        /// Handles the cancel job event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelJob(CancelJobEvent e, IContext context)
        {
            this.Logger.Info("Cancelling job with ID '{jobId}'...", e.JobId);

            if (!this.activeJobs.TryRemove(e.JobId, out var tuple))
            {
                this.Logger.Warn(
                    "Job with ID '{jobId}' was already removed from the list of active jobs.",
                    e.JobId);
            }

            tuple.cancellationSource.Cancel();

            this.Logger.Info("Job with ID '{jobId}' was signaled for cancellation.", e.JobId);
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
            var job = (IJob)jobInfo.CreateInstance();
            job.Target = target;
            job.Arguments = arguments;

            return new JobResult(
                job.Id,
                this.workflowProcessor.ExecuteAsync(job, target, arguments, options, cancellationToken))
            {
                Job = job,
            };
        }
    }
}