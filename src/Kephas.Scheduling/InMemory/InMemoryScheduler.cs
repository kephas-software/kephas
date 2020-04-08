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
            this.subscriptions.ForEach(s => s.Dispose());

            // stop triggers
            while (this.activeTriggers.Count > 0)
            {
                var kv = this.activeTriggers.FirstOrDefault();
                if (kv.Key != null)
                {
                    kv.Value.trigger.Dispose();
                    this.activeTriggers.TryRemove(kv.Key, out _);
                }
            }

            // stop jobs
            while (this.activeJobs.Count > 0)
            {
                var kv = this.activeJobs.FirstOrDefault();
                if (kv.Key != null)
                {
                    kv.Value.cancellationSource.Cancel();
                    this.activeJobs.TryRemove(kv.Key, out _);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the enqueue event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleEnqueue(EnqueueEvent e, IContext context)
        {
            if (!(e.JobInfo is IJobInfo jobInfoObject))
            {
                throw new ArgumentException(
                    $"Only {nameof(IJobInfo)} values accepted for the {nameof(e.JobInfo)} parameter.",
                    nameof(e.JobInfo));
            }

            var activityContext = this.CreateActivityContext(e.Options);

            var trigger = activityContext.Trigger()
                          ?? new TimerTrigger(e.TriggerId ?? Guid.NewGuid());

            if (!this.activeTriggers.TryAdd(
                trigger.Id,
                (trigger, ct => this.StartJob(jobInfoObject, e.Target, e.Arguments, e.Options, ct))))
            {
                this.Logger.Warn("Cannot enqueue trigger with ID {triggerId}.", trigger.Id);
                return;
            }

            void OnTriggerOnFire(object sender, EventArgs args)
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
                    this.Logger.Warn("Cannot add the job with ID '{jobId}' to the list of active jobs.",
                        jobResult.JobId);
                }
            }

            void OnTriggerOnDisposed(object sender, EventArgs args)
            {
                var triggerId = trigger.Id;
                this.activeTriggers.TryRemove(triggerId, out _);
                trigger.Fire -= OnTriggerOnFire;
                trigger.Disposed -= OnTriggerOnDisposed;
            }

            trigger.Fire += OnTriggerOnFire;
            trigger.Disposed += OnTriggerOnDisposed;

            ServiceHelper.Initialize(trigger);
        }

        /// <summary>
        /// Handles the cancel trigger event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelTrigger(CancelTriggerEvent e, IContext context)
        {
            if (!this.activeTriggers.TryRemove(e.TriggerId, out var tuple))
            {
                this.Logger.Warn("Trigger with ID '{triggerId}' was already removed from the list of active triggers.",
                    e.TriggerId);
            }

            tuple.trigger.Dispose();
        }

        /// <summary>
        /// Handles the cancel job event.
        /// </summary>
        /// <param name="e">The event to process.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCancelJob(CancelJobEvent e, IContext context)
        {
            if (!this.activeJobs.TryRemove(e.JobId, out var tuple))
            {
                this.Logger.Warn("Job with ID '{jobId}' was already removed from the list of active triggers.",
                    e.JobId);
            }

            tuple.cancellationSource.Cancel();
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