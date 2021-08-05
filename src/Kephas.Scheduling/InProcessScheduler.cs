// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessScheduler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory scheduler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.JobStore;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow;

    /// <summary>
    /// A scheduler processing jobs in the executing process.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InProcessScheduler : Loggable, IScheduler
    {
        private readonly IContextFactory contextFactory;
        private readonly IWorkflowProcessor workflowProcessor;
        private readonly IJobStore jobStore;
        private readonly IAppRuntime appRuntime;

        private readonly ConcurrentDictionary<object, (ITrigger trigger, Func<CancellationTokenSource, IJobResult> triggerAction)>
            activeTriggers = new ConcurrentDictionary<object, (ITrigger trigger, Func<CancellationTokenSource, IJobResult> triggerAction)>();

        private readonly FinalizationMonitor<IScheduler> finalizationMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessScheduler"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="workflowProcessor">The workflow processor.</param>
        /// <param name="jobStore">The job store.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="logManager">The log manager.</param>
        public InProcessScheduler(
            IContextFactory contextFactory,
            IWorkflowProcessor workflowProcessor,
            IJobStore jobStore,
            IAppRuntime appRuntime,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.contextFactory = contextFactory;
            this.workflowProcessor = workflowProcessor;
            this.jobStore = jobStore;
            this.appRuntime = appRuntime;
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
        public async Task FinalizeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.finalizationMonitor.Start();

            // cancel triggers
            while (this.activeTriggers.Count > 0)
            {
                var kv = this.activeTriggers.FirstOrDefault();
                if (kv.Key != null)
                {
                    await this.CancelTriggerAsync(kv.Key, ctx => ctx.Impersonate(context), cancellationToken).PreserveThreadContext();
                }
            }

            // cancel running jobs
            var runningJobs = this.jobStore.GetRunningJobs().ToList();
            foreach (var runningJob in runningJobs)
            {
                await this.CancelRunningJobAsync(
                    runningJob,
                    ctx => ctx.Impersonate(context),
                    cancellationToken).PreserveThreadContext();
            }

            // cancel scheduled jobs, if any.
            var scheduledJobs = this.jobStore.GetScheduledJobs().ToList();
            foreach (var scheduledJob in scheduledJobs)
            {
                await this.CancelScheduledJobAsync(scheduledJob, ctx => ctx.Impersonate(context), cancellationToken).PreserveThreadContext();
            }

            this.finalizationMonitor.Complete();
        }

        /// <summary>
        /// Gets the scheduled jobs.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>A query over the scheduled jobs.</returns>
        public IQueryable<IJobInfo> GetScheduledJobs(Action<ISchedulingContext>? options = null) => this.jobStore.GetScheduledJobs();

        /// <summary>
        /// Gets the running jobs.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>A query over the running jobs.</returns>
        public IQueryable<IJobResult> GetRunningJobs(Action<ISchedulingContext>? options = null) => this.jobStore.GetRunningJobs();

        /// <summary>
        /// Gets the completed jobs.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>A query over the completed jobs.</returns>
        public IQueryable<IJobResult> GetCompletedJobs(Action<ISchedulingContext>? options = null) => this.jobStore.GetCompletedJobs();

        /// <summary>
        /// Disables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        public Task<IOperationResult<IJobInfo?>> DisableScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
            => this.ToggleScheduledJobAsync(scheduledJob, false, options, cancellationToken);

        /// <summary>
        /// Enables all the triggers of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        public virtual Task<IOperationResult<IJobInfo?>> EnableScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
            => this.ToggleScheduledJobAsync(scheduledJob, true, options, cancellationToken);

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job instance.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IJobInfo?>> CancelScheduledJobAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            using var schedulingContext = this.CreateSchedulingContext(options);

            this.Logger.Info("Cancelling jobs and triggers based on scheduled job '{scheduledJob}'...", scheduledJob);

            var jobInfo = scheduledJob as IJobInfo
                          ?? await this.jobStore.GetScheduledJobAsync(scheduledJob!, false, cancellationToken).PreserveThreadContext();

            if (jobInfo == null)
            {
                this.Logger.Info("No scheduled jobs found for '{scheduledJob}'.", scheduledJob);
                return jobInfo.ToOperationResult()
                    .Fail(new SchedulingException($"No scheduled jobs found for '{scheduledJob}'."));
            }

            // first of all cancel all matching triggers...
            // make a copy of the Triggers collection as CancelTriggerAsync removes it from the collection.
            foreach (var trigger in jobInfo.Triggers.ToArray())
            {
                await this.CancelTriggerAsync(trigger.Id, options, cancellationToken).PreserveThreadContext();
            }

            // ...then all matching running jobs...
            var matchingRunningJobs = this.jobStore.GetRunningJobs()
                .Where(job =>
                    jobInfo.Id.Equals(job.ScheduledJobId)
                    || jobInfo.Equals(job.ScheduledJob))
                .ToArray();
            if (matchingRunningJobs.Length == 0)
            {
                this.Logger.Info("No running jobs found for '{scheduledJob}'.", scheduledJob);
            }
            else
            {
                foreach (var job in matchingRunningJobs)
                {
                    await this.CancelRunningJobAsync(job, options, cancellationToken).PreserveThreadContext();
                }
            }

            // ... and last remove the scheduled job.
            await this.jobStore.RemoveScheduledJobAsync(jobInfo.Id, cancellationToken).PreserveThreadContext();
            return jobInfo.ToOperationResult()!;
        }

        /// <summary>
        /// Cancels the running job asynchronously.
        /// </summary>
        /// <param name="runningJob">The identifier of the running job.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IJobResult?>> CancelRunningJobAsync(
            object runningJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            var job = runningJob as IJobResult
                      ?? await this.jobStore.GetRunningJobAsync(runningJob, false, cancellationToken)
                          .PreserveThreadContext();

            if (job == null)
            {
                this.Logger.Info("Running job '{job}' not found, probably completed or already canceled.", runningJob);
                return job.ToOperationResult().Fail(new SchedulingException($"Running job '{runningJob}' not found, probably completed or already canceled."));
            }

            this.Logger.Info("Cancelling running job '{job}'...", job);

            await job.Cancel().AsTask().PreserveThreadContext();
            await this.jobStore.RemoveRunningJobAsync(job.RunningJobId!, cancellationToken).PreserveThreadContext();

            this.Logger.Info("Job '{job}' was removed from the list of running jobs and was signaled for cancellation.", job);
            return job.ToOperationResult()
                .MergeMessage($"Job '{job}' was removed from the list of running jobs and was signaled for cancellation.")!;
        }

        /// <summary>
        /// Cancels the trigger asynchronously.
        /// </summary>
        /// <param name="trigger">The trigger to be canceled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public virtual async Task<IOperationResult<ITrigger?>> CancelTriggerAsync(
            object trigger,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(trigger, nameof(trigger));

            this.Logger.Info("Cancelling trigger '{trigger}'...", trigger);

            var triggerId = trigger is ITrigger triggerInstance ? triggerInstance.Id : trigger;
            if (!this.activeTriggers.TryRemove(triggerId, out var tuple))
            {
                this.Logger.Warn("Trigger '{trigger}' was already removed from the list of active triggers.", trigger);
                return ((ITrigger?)null).ToOperationResult()
                    .Fail(new SchedulingException($"Trigger '{trigger}' was already removed from the list of active triggers."));
            }

            await this.jobStore.SetTriggerEnabledAsync(tuple.trigger, false, cancellationToken).PreserveThreadContext();
            tuple.trigger.Dispose();
            await this.jobStore.RemoveTriggerAsync(tuple.trigger.Id, cancellationToken).PreserveThreadContext();

            this.Logger.Info("Trigger '{trigger}' was removed from the list of active triggers and was canceled.", tuple.trigger);
            return tuple.trigger.ToOperationResult()
                .MergeMessage($"Trigger '{tuple.trigger}' was removed from the list of active triggers and was canceled.")!;
        }

        /// <summary>
        /// Enqueues a new job using a job definition or its ID.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual async Task<IOperationResult<IJobInfo?>> EnqueueAsync(
            object scheduledJob,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            if (!(scheduledJob is IJobInfo jobInfo))
            {
                throw new ArgumentException($"The '{nameof(scheduledJob)}' must be of type '{typeof(IJobInfo)}'.");
            }

            if (!this.finalizationMonitor.IsNotStarted)
            {
                this.Logger.Warn("Finalization is started, enqueue requests are rejected.");
                return jobInfo.ToOperationResult()
                    .Fail(new InvalidOperationException("Finalization is started, enqueue requests are rejected."))!;
            }

            var schedulingContext = this.CreateSchedulingContext(options);

            await this.jobStore.AddScheduledJobAsync(jobInfo, cancellationToken).PreserveThreadContext();
            var trigger = schedulingContext.Trigger()
                          ?? new TimerTrigger(Guid.NewGuid());

            await this.jobStore.AddTriggerAsync(trigger, jobInfo, cancellationToken).PreserveThreadContext();

            if (!this.activeTriggers.TryAdd(
                trigger.Id,
                (trigger, cts => this.StartJob(jobInfo, schedulingContext.ActivityTarget, schedulingContext.ActivityArguments, schedulingContext.ActivityOptions, cts))))
            {
                this.Logger.Warn("Cannot enqueue trigger '{trigger}' ({triggerId}).", trigger, trigger.Id);
                return jobInfo.ToOperationResult()
                    .Fail(new SchedulingException($"Cannot enqueue trigger '{trigger}' ({trigger.Id})."))!;
            }

            async Task OnTriggerOnFireAsync(object sender, FireEventArgs args)
            {
                var triggerId = trigger.Id;
                if (!this.activeTriggers.TryGetValue(triggerId, out var tuple))
                {
                    this.Logger.Warn("Trigger with ID {triggerId} not found.", triggerId);
                    return;
                }

                var cancellationSource = new CancellationTokenSource();
                var runningJob = tuple.triggerAction(cancellationSource);
                var startedAt = runningJob.StartedAt ?? DateTimeOffset.Now;
                await this.jobStore.AddRunningJobAsync(runningJob).PreserveThreadContext();

                // upon completion, remove the job from the running collection.
                var runningJobTask = runningJob.AsTask();
                try
                {
                    await runningJobTask.PreserveThreadContext();

                    var endedAt = DateTimeOffset.Now;
                    runningJob.Elapsed = endedAt - startedAt;
                    runningJob.OperationState = OperationState.Completed;
                    if (runningJob is JobResult jobResult)
                    {
                        jobResult.EndedAt = endedAt;
                        jobResult.PercentCompleted = 1;
                    }
                }
                catch (Exception ex)
                {
                    (runningJob.Logger ?? this.Logger).Error(ex, $"Errors occurred while running job '{runningJob}'");
                    runningJob.Fail(ex, DateTimeOffset.Now - startedAt);
                }

                try
                {
                    // also, upon completion, remove the running job from the store and added to the completed collection.
                    await this.jobStore.RemoveRunningJobAsync(runningJob.RunningJobId!).PreserveThreadContext();
                    await this.jobStore.AddCompletedJobResultAsync(runningJob).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Errors occured while moving the job result from the running to completed collection.");
                }

                try
                {
                    args.CompleteCallback?.Invoke(runningJob);
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Errors occured while invoking the callback.");
                }
            }

            void OnTriggerOnFire(object sender, FireEventArgs args)
            {
#pragma warning disable 4014
                OnTriggerOnFireAsync(sender, args);
#pragma warning restore 4014
            }

            async Task OnTriggerOnDisposedAsync(object sender, EventArgs args)
            {
                var triggerId = trigger!.Id;
                if (this.activeTriggers.TryRemove(triggerId, out _))
                {
                    trigger.Fire -= OnTriggerOnFire;
                    trigger.Disposed -= OnTriggerOnDisposed;
                    await this.jobStore.RemoveTriggerAsync(triggerId).PreserveThreadContext();
                    if (!jobInfo!.Triggers.Any())
                    {
                        await this.jobStore.RemoveScheduledJobAsync(jobInfo.Id).PreserveThreadContext();
                    }
                }

                schedulingContext?.Dispose();
            }

            void OnTriggerOnDisposed(object sender, EventArgs args)
            {
#pragma warning disable 4014
                OnTriggerOnDisposedAsync(sender, args);
#pragma warning restore 4014
            }

            trigger.Fire += OnTriggerOnFire;
            trigger.Disposed += OnTriggerOnDisposed;

            this.Logger.Info("Enqueued job '{scheduledJob}' with trigger '{trigger}'.", jobInfo, trigger);

            await ServiceHelper.InitializeAsync(trigger, cancellationToken: cancellationToken).PreserveThreadContext();
            return jobInfo.ToOperationResult()!;
        }

        /// <summary>
        /// Depending on the <paramref name="enabled"/> argument, enables or disables all the triggers
        /// of the scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled <see cref="IJobInfo"/> to be disabled or its ID.</param>
        /// <param name="enabled">Indicates whether to enable or to disable the scheduled job.</param>
        /// <param name="options">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The asynchronous result yielding an operation result.</returns>
        protected virtual async Task<IOperationResult<IJobInfo?>> ToggleScheduledJobAsync(
            object scheduledJob,
            bool enabled,
            Action<ISchedulingContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(scheduledJob, nameof(scheduledJob));

            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            var jobInfo = scheduledJob as IJobInfo
                          ?? await this.jobStore.GetScheduledJobAsync(scheduledJob!, false, cancellationToken).PreserveThreadContext();

            if (jobInfo == null)
            {
                return jobInfo.ToOperationResult()
                    .Fail(new KeyNotFoundException($"Scheduled job '{scheduledJob}' was not found."));
            }

            foreach (var trigger in jobInfo.Triggers.ToArray())
            {
                await this.jobStore.SetTriggerEnabledAsync(trigger, enabled, cancellationToken).PreserveThreadContext();
            }

            var enableString = enabled ? "enabled" : "disabled";
            return jobInfo.ToOperationResult()
                .MergeMessage($"Scheduled job '{scheduledJob}' was {enableString}.")!;
        }

        /// <summary>
        /// Starts the operation associated to the scheduled job.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="target">The target.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <returns>The running job result.</returns>
        protected virtual IJobResult StartJob(
            IJobInfo scheduledJob,
            object? target,
            IDynamic? arguments,
            Action<IActivityContext>? options,
            CancellationTokenSource cancellationTokenSource)
        {
            var job = (IJob)scheduledJob.CreateInstance();
            job.Target = target;
            job.Arguments = arguments;
            var startedAt = DateTimeOffset.Now;

            return new JobResult(
                job.Id,
                this.workflowProcessor.ExecuteAsync(job, target, arguments, options, cancellationTokenSource.Token))
            {
                ScheduledJob = scheduledJob,
                RunningJob = job,
                StartedAt = startedAt,
                OperationState = OperationState.InProgress,
                CancellationTokenSource = cancellationTokenSource,
                Logger = this.Logger,
                AppInstanceId = this.appRuntime.GetAppInstanceId(),
            };
        }

        /// <summary>
        /// Creates the scheduling context.
        /// </summary>
        /// <param name="options">Optional. The options configuration.</param>
        /// <returns>
        /// The new scope context.
        /// </returns>
        protected virtual ISchedulingContext CreateSchedulingContext(Action<ISchedulingContext>? options = null)
        {
            return this.contextFactory.CreateContext<SchedulingContext>().Merge(options);
        }
    }
}