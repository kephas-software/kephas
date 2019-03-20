// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingJobStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scheduling job store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Quartz;
    using global::Quartz.Impl.AdoJobStore;
    using global::Quartz.Impl.Matchers;
    using global::Quartz.Spi;
    using global::Quartz.Util;

    using Kephas.Data;
    using Kephas.Data.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Scheduling.Quartz.JobStore.Repositories;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using TriggerState = global::Quartz.TriggerState;

    /// <summary>
    /// A scheduling job store.
    /// </summary>
    public class SchedulingJobStore : ISchedulingJobStore
    {
        private const string KeySignalChangeForTxCompletion = "sigChangeForTxCompletion";
        private const string AllGroupsPaused = "_$_ALL_GROUPS_PAUSED_$_";

        private static readonly DateTimeOffset? SchedulingSignalDateTime = new DateTimeOffset(1982, 6, 28, 0, 0, 0, TimeSpan.FromSeconds(0));
        private static long fireTriggerRecordCounter = DateTime.UtcNow.Ticks;

        private readonly ITriggerFactory triggerFactory;

        private TimeSpan misfireThreshold = TimeSpan.FromMinutes(1);

        private ISchedulerSignaler schedulerSignaler;

        private MisfireHandler misfireHandler;
        private bool schedulerRunning;

        private LockManager lockManager;

        private TriggerRepository triggerRepository;

        /// <summary>
        /// The data context factory.
        /// </summary>
        private Func<IContext, IDataContext> dataContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingJobStore"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public SchedulingJobStore(ITriggerFactory triggerFactory, ILogManager logManager)
        {
            this.MaxMisfiresToHandleAtATime = 20;
            this.RetryableActionErrorLogThreshold = 4;
            this.DbRetryInterval = TimeSpan.FromSeconds(15);

            this.triggerFactory = triggerFactory;
            this.LogManager = logManager;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<SchedulingJobStore> Logger { get; set; }

        /// <summary>
        /// Gets the data context factory.
        /// </summary>
        /// <value>
        /// The data context factory.
        /// </value>
        public Func<IContext, IDataContext> DataContextFactory
        {
            get
            {
                if (this.dataContextFactory == null)
                {
                    // TODO localization
                    throw new ServiceNotInitializedException($"The {typeof(SchedulingJobStore)} service is not initialized.");
                }

                return this.dataContextFactory;
            }
        }

        /// <summary>
        /// Gets a value indicating whether job store supports persistence.
        /// </summary>
        /// <value>
        /// True if supports persistence, false if not.
        /// </value>
        public bool SupportsPersistence => true;

        /// <summary>
        /// Gets a value indicating how long (in milliseconds) the <see cref="T:Quartz.Spi.IJobStore" /> implementation
        /// estimates that it will take to release a trigger and acquire a new one.
        /// </summary>
        public long EstimatedTimeToReleaseAndAcquireTrigger => 200;

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="T:Quartz.Spi.IJobStore" /> implementation is clustered.
        /// </summary>
        public bool Clustered => false;

        /// <summary>
        /// Gets or sets a value informing the <see cref="T:Quartz.Spi.IJobStore" /> of the Scheduler instance's Id,
        /// prior to initialize being invoked.
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets a value informing the <see cref="T:Quartz.Spi.IJobStore" /> of the Scheduler instance's name,
        /// prior to initialize being invoked.
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Gets a value telling the JobStore the pool size used to execute jobs.
        /// </summary>
        public int ThreadPoolSize { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of misfired triggers that the misfire handling
        ///     thread will try to recover at one time (within one transaction).  The
        ///     default is 20.
        /// </summary>
        public int MaxMisfiresToHandleAtATime { get; set; }

        /// <summary>
        ///     Gets or sets the database retry interval.
        /// </summary>
        /// <value>The db retry interval.</value>
        [TimeSpanParseRule(TimeSpanParseRule.Milliseconds)]
        public TimeSpan DbRetryInterval { get; set; }

        /// <summary>
        ///     Gets or sets the time span by which a trigger must have missed its
        ///     next-fire-time, in order for it to be considered "misfired" and thus
        ///     have its misfire instruction applied.
        /// </summary>
        [TimeSpanParseRule(TimeSpanParseRule.Milliseconds)]
        public TimeSpan MisfireThreshold
        {
            get => this.misfireThreshold;
            set
            {
                if (value.TotalMilliseconds < 1)
                {
                    throw new ArgumentException("MisfireThreshold must be larger than 0");
                }

                this.misfireThreshold = value;
            }
        }

        /// <summary>
        ///     Gets or sets the number of retries before an error is logged for recovery operations.
        /// </summary>
        public int RetryableActionErrorLogThreshold { get; set; }

        /// <summary>
        /// Gets the manager for log.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        internal ILogManager LogManager { get; }

        /// <summary>
        /// Gets the misfire time.
        /// </summary>
        /// <value>
        /// The misfire time.
        /// </value>
        protected DateTimeOffset MisfireTime
        {
            get
            {
                var misfireTime = SystemTime.UtcNow();
                if (this.MisfireThreshold > TimeSpan.Zero)
                {
                    misfireTime = misfireTime.AddMilliseconds(-1 * this.MisfireThreshold.TotalMilliseconds);
                }

                return misfireTime;
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Scheduler {this.InstanceId}/{this.InstanceName}";
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void IInitializable.Initialize(IContext context)
        {
            if (!(context is ISchedulingJobStoreContext jobStoreContext))
            {
                // TODO localization
                throw new InvalidOperationException(
                    $"The context provided must be a {typeof(ISchedulingJobStoreContext)}.");
            }

            Requires.NotNull(jobStoreContext.DataContextFactory, nameof(jobStoreContext.DataContextFactory));

            this.dataContextFactory = jobStoreContext.DataContextFactory;
        }

        /// <summary>
        /// Called by the QuartzScheduler before the <see cref="T:Quartz.Spi.IJobStore" /> is used, in
        /// order to give the it a chance to Initialize.
        /// </summary>
        /// <param name="loadHelper">The load helper.</param>
        /// <param name="signaler">The signaler.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task IJobStore.Initialize(
            ITypeLoadHelper loadHelper,
            ISchedulerSignaler signaler,
            CancellationToken cancellationToken = default)
        {
            this.schedulerSignaler = signaler;

            this.Logger.Trace($"Scheduler {this} initialize.");

            this.lockManager = new LockManager(this.InstanceName);

            this.triggerRepository = new TriggerRepository(this);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Called by the QuartzScheduler to inform the <see cref="T:Quartz.Spi.IJobStore" /> that
        /// the scheduler has started.
        /// </summary>
        public async Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            this.Logger.Trace($"Scheduler {this} started.");
            using (var dataContext = this.DataContextFactory(null))
            {
                await dataContext.AddScheduler(this.InstanceName, this.InstanceId, cancellationToken).PreserveThreadContext();

                try
                {
                    await this.RecoverJobs(dataContext, cancellationToken).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    throw new SchedulerConfigException("Failure occurred during job recovery", ex);
                }
            }

            this.misfireHandler = new MisfireHandler(this);
            this.misfireHandler.Start();
            this.schedulerRunning = true;
        }

        /// <summary>
        /// Called by the QuartzScheduler to inform the JobStore that
        /// the scheduler has been paused.
        /// </summary>
        public async Task SchedulerPaused(CancellationToken cancellationToken = default)
        {
            this.Logger.Trace($"Scheduler {this} paused.");

            using (var dataContext = this.DataContextFactory(null))
            {
                await dataContext.UpdateSchedulerState(
                    this.InstanceName,
                    this.InstanceId,
                    SchedulerState.Paused,
                    cancellationToken).PreserveThreadContext();
            }

            this.schedulerRunning = false;
        }

        /// <summary>
        /// Called by the QuartzScheduler to inform the JobStore that
        /// the scheduler has resumed after being paused.
        /// </summary>
        public async Task SchedulerResumed(CancellationToken cancellationToken = default)
        {
            this.Logger.Trace($"Scheduler {this} resumed.");
            using (var dataContext = this.DataContextFactory(null))
            {
                await dataContext.UpdateSchedulerState(
                    this.InstanceName,
                    this.InstanceId,
                    SchedulerState.Resumed,
                    cancellationToken).PreserveThreadContext();
            }

            this.schedulerRunning = true;
        }

        /// <summary>
        /// Called by the QuartzScheduler to inform the <see cref="T:Quartz.Spi.IJobStore" /> that
        /// it should free up all of it's resources because the scheduler is
        /// shutting down.
        /// </summary>
        public async Task Shutdown(CancellationToken cancellationToken = default)
        {
            this.Logger.Trace($"Scheduler {this} shutdown");
            if (this.misfireHandler != null)
            {
                this.misfireHandler.Shutdown();
                try
                {
                    this.misfireHandler.Join();
                }
                catch (ThreadInterruptedException)
                {
                }
            }

            using (var dataContext = this.DataContextFactory(null))
            {
                await dataContext.DeleteScheduler(this.InstanceName, this.InstanceId, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Store the given <see cref="T:Quartz.IJobDetail" /> and <see cref="T:Quartz.ITrigger" />.
        /// </summary>
        /// <param name="newJob">The <see cref="T:Quartz.IJobDetail" /> to be stored.</param>
        /// <param name="newTrigger">The <see cref="T:Quartz.ITrigger" /> to be stored.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <throws>  ObjectAlreadyExistsException </throws>
        public async Task StoreJobAndTrigger(
            global::Quartz.IJobDetail newJob,
            IOperableTrigger newTrigger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.StoreJobInternal(dataContext, newJob, false, cancellationToken).PreserveThreadContext();
                    await this.StoreTriggerInternal(
                        dataContext,
                        newTrigger,
                        newJob,
                        false,
                        Model.TriggerState.Waiting,
                        false,
                        false,
                        cancellationToken).PreserveThreadContext();
                }
            }
            catch (AggregateException ex)
            {
                throw new JobPersistenceException(ex.InnerExceptions[0].Message, ex.InnerExceptions[0]);
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>returns true if the given JobGroup is paused</summary>
        /// <param name="groupName"></param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public async Task<bool> IsJobGroupPaused(string groupName, CancellationToken cancellationToken = default)
        {
            // This is not implemented in the core ADO stuff, so we won't implement it here either
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns true if the given TriggerGroup
        /// is paused
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public async Task<bool> IsTriggerGroupPaused(string groupName, CancellationToken cancellationToken = default)
        {
            // This is not implemented in the core ADO stuff, so we won't implement it here either
            throw new NotImplementedException();
        }

        /// <summary>
        /// Store the given <see cref="T:Quartz.IJobDetail" />.
        /// </summary>
        /// <param name="newJob">The <see cref="T:Quartz.IJobDetail" /> to be stored.</param>
        /// <param name="replaceExisting">
        /// If <see langword="true" />, any <see cref="T:Quartz.IJob" /> existing in the
        /// <see cref="T:Quartz.Spi.IJobStore" /> with the same name and group should be
        /// over-written.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        public async Task StoreJob(global::Quartz.IJobDetail newJob, bool replaceExisting, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.StoreJobInternal(dataContext, newJob, replaceExisting, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        public async Task StoreJobsAndTriggers(
            IReadOnlyDictionary<global::Quartz.IJobDetail, IReadOnlyCollection<global::Quartz.ITrigger>> triggersAndJobs,
            bool replace,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    foreach (var job in triggersAndJobs.Keys)
                    {
                        await this.StoreJobInternal(dataContext, job, replace, cancellationToken).PreserveThreadContext();
                        foreach (var trigger in triggersAndJobs[job])
                        {
                            await this.StoreTriggerInternal(
                                dataContext,
                                (IOperableTrigger)trigger,
                                job,
                                replace,
                                Model.TriggerState.Waiting,
                                false,
                                false,
                                cancellationToken).PreserveThreadContext();
                        }
                    }
                }
            }
            catch (AggregateException ex)
            {
                throw new JobPersistenceException(ex.InnerExceptions[0].Message, ex.InnerExceptions[0]);
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove (delete) the <see cref="T:Quartz.IJob" /> with the given
        /// key, and any <see cref="T:Quartz.ITrigger" /> s that reference
        /// it.
        /// </summary>
        /// <remarks>
        /// If removal of the <see cref="T:Quartz.IJob" /> results in an empty group, the
        /// group should be removed from the <see cref="T:Quartz.Spi.IJobStore" />'s list of
        /// known group names.
        /// </remarks>
        /// <returns>
        /// 	<see langword="true" /> if a <see cref="T:Quartz.IJob" /> with the given name and
        /// group was found and removed from the store.
        /// </returns>
        public async Task<bool> RemoveJob(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.RemoveJobInternal(dataContext, jobKey, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        public async Task<bool> RemoveJobs(IReadOnlyCollection<JobKey> jobKeys, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    var results =
                        await Task.WhenAll(jobKeys.Select(jobKey => this.RemoveJobInternal(dataContext, jobKey, cancellationToken)))
                            .PreserveThreadContext();
                    return results.Aggregate(true, (current, result) => current && result);
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieve the <see cref="T:Quartz.IJobDetail" /> for the given
        /// <see cref="T:Quartz.IJob" />.
        /// </summary>
        /// <returns>
        /// The desired <see cref="T:Quartz.IJob" />, or null if there is no match.
        /// </returns>
        public async Task<global::Quartz.IJobDetail> RetrieveJob(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return await dataContext.GetJobDetail(this.InstanceName, jobKey, cancellationToken)
                           .PreserveThreadContext();
            }
        }

        /// <summary>
        /// Store the given <see cref="T:Quartz.ITrigger" />.
        /// </summary>
        /// <param name="newTrigger">The <see cref="T:Quartz.ITrigger" /> to be stored.</param>
        /// <param name="replaceExisting">If <see langword="true" />, any <see cref="T:Quartz.ITrigger" /> existing in
        /// the <see cref="T:Quartz.Spi.IJobStore" /> with the same name and group should
        /// be over-written.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <throws>  ObjectAlreadyExistsException </throws>
        public async Task StoreTrigger(
            IOperableTrigger newTrigger,
            bool replaceExisting,
            CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
            {
                await this.StoreTriggerInternal(
                    dataContext,
                    newTrigger,
                    null,
                    replaceExisting,
                    Model.TriggerState.Waiting,
                    false,
                    false,
                    cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Remove (delete) the <see cref="T:Quartz.ITrigger" /> with the given key.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If removal of the <see cref="T:Quartz.ITrigger" /> results in an empty group, the
        /// group should be removed from the <see cref="T:Quartz.Spi.IJobStore" />'s list of
        /// known group names.
        /// </para>
        /// <para>
        /// If removal of the <see cref="T:Quartz.ITrigger" /> results in an 'orphaned' <see cref="T:Quartz.IJob" />
        /// that is not 'durable', then the <see cref="T:Quartz.IJob" /> should be deleted
        /// also.
        /// </para>
        /// </remarks>
        /// <returns>
        /// 	<see langword="true" /> if a <see cref="T:Quartz.ITrigger" /> with the given
        /// name and group was found and removed from the store.
        /// </returns>
        public async Task<bool> RemoveTrigger(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.RemoveTriggerInternal(dataContext, triggerKey, cancellationToken: cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        public async Task<bool> RemoveTriggers(IReadOnlyCollection<TriggerKey> triggerKeys, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    var results = await Task.WhenAll(
                        triggerKeys.Select(
                            triggerKey => this.RemoveTriggerInternal(
                                dataContext,
                                triggerKey,
                                cancellationToken: cancellationToken))).PreserveThreadContext();

                    return results.Aggregate(true,
                        (current, result) => current && result);
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove (delete) the <see cref="T:Quartz.ITrigger" /> with the
        /// given name, and store the new given one - which must be associated
        /// with the same job.
        /// </summary>
        /// <param name="triggerKey">The <see cref="T:Quartz.ITrigger" /> to be replaced.</param>
        /// <param name="newTrigger">The new <see cref="T:Quartz.ITrigger" /> to be stored.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>
        /// 	<see langword="true" /> if a <see cref="T:Quartz.ITrigger" /> with the given
        /// name and group was found and removed from the store.
        /// </returns>
        public async Task<bool> ReplaceTrigger(
            TriggerKey triggerKey,
            IOperableTrigger newTrigger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.ReplaceTriggerInternal(dataContext, triggerKey, newTrigger, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieve the given <see cref="T:Quartz.ITrigger" />.
        /// </summary>
        /// <returns>
        /// The desired <see cref="T:Quartz.ITrigger" />, or null if there is no
        /// match.
        /// </returns>
        public async Task<IOperableTrigger> RetrieveTrigger(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            var result = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();
            return result?.GetTrigger() as IOperableTrigger;
        }

        /// <summary>
        /// Determine whether a <see cref="T:Quartz.ICalendar" /> with the given identifier already
        /// exists within the scheduler.
        /// </summary>
        /// <param name="calName">the identifier to check for</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>true if a calendar exists with the given identifier</returns>
        public async Task<bool> CalendarExists(string calName, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return await dataContext
                           .CalendarExists(this.InstanceName, calName, cancellationToken: cancellationToken)
                           .PreserveThreadContext();
            }
        }

        /// <summary>
        /// Determine whether a <see cref="T:Quartz.IJob" /> with the given identifier already
        /// exists within the scheduler.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="jobKey">the identifier to check for</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>true if a job exists with the given identifier</returns>
        public async Task<bool> CheckExists(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return await dataContext
                           .JobExists(this.InstanceName, jobKey, cancellationToken: cancellationToken)
                           .PreserveThreadContext();
            }
        }

        /// <summary>
        /// Determine whether a <see cref="T:Quartz.ITrigger" /> with the given identifier already
        /// exists within the scheduler.
        /// </summary>
        /// <param name="triggerKey">the identifier to check for</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>true if a trigger exists with the given identifier</returns>
        public Task<bool> CheckExists(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            return this.triggerRepository.TriggerExists(triggerKey, cancellationToken);
        }

        /// <summary>
        /// Clear (delete!) all scheduling data - all <see cref="T:Quartz.IJob" />s, <see cref="T:Quartz.ITrigger" />s
        /// <see cref="T:Quartz.ICalendar" />s.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public async Task ClearAllSchedulingData(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await dataContext.BulkDeleteAsync<Model.ICalendar>(c => true, cancellationToken: cancellationToken).PreserveThreadContext();
                    await dataContext.BulkDeleteAsync<Model.IFiredTrigger>(c => true, cancellationToken: cancellationToken).PreserveThreadContext();
                    await dataContext.BulkDeleteAsync<Model.IJobDetail>(c => true, cancellationToken: cancellationToken).PreserveThreadContext();
                    await dataContext.BulkDeleteAsync<Model.IPausedTriggerGroup>(c => true, cancellationToken: cancellationToken).PreserveThreadContext();
                    await dataContext.BulkDeleteAsync<Model.IScheduler>(c => true, cancellationToken: cancellationToken).PreserveThreadContext();
                    await dataContext.BulkDeleteAsync<Model.ITrigger>(c => true, cancellationToken: cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Store the given <see cref="T:Quartz.ICalendar" />.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="calendar">The <see cref="T:Quartz.ICalendar" /> to be stored.</param>
        /// <param name="replaceExisting">If <see langword="true" />, any <see cref="T:Quartz.ICalendar" /> existing
        /// in the <see cref="T:Quartz.Spi.IJobStore" /> with the same name and group
        /// should be over-written.</param>
        /// <param name="updateTriggers">If <see langword="true" />, any <see cref="T:Quartz.ITrigger" />s existing
        /// in the <see cref="T:Quartz.Spi.IJobStore" /> that reference an existing
        /// Calendar with the same name with have their next fire time
        /// re-computed with the new <see cref="T:Quartz.ICalendar" />.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <throws>  ObjectAlreadyExistsException </throws>
        public async Task StoreCalendar(
            string name,
            global::Quartz.ICalendar calendar,
            bool replaceExisting,
            bool updateTriggers,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.StoreCalendarInternal(dataContext, name, calendar, replaceExisting, updateTriggers, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove (delete) the <see cref="T:Quartz.ICalendar" /> with the
        /// given name.
        /// </summary>
        /// <remarks>
        /// If removal of the <see cref="T:Quartz.ICalendar" /> would result in
        /// <see cref="T:Quartz.ITrigger" />s pointing to non-existent calendars, then a
        /// <see cref="T:Quartz.JobPersistenceException" /> will be thrown.
        /// </remarks>
        /// <param name="calName">The name of the <see cref="T:Quartz.ICalendar" /> to be removed.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>
        /// 	<see langword="true" /> if a <see cref="T:Quartz.ICalendar" /> with the given name
        /// was found and removed from the store.
        /// </returns>
        public async Task<bool> RemoveCalendar(string calName, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.RemoveCalendarInternal(dataContext, calName, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieve the given <see cref="T:Quartz.ITrigger" />.
        /// </summary>
        /// <param name="calName">The name of the <see cref="T:Quartz.ICalendar" /> to be retrieved.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>
        /// The desired <see cref="T:Quartz.ICalendar" />, or null if there is no
        /// match.
        /// </returns>
        public async Task<global::Quartz.ICalendar> RetrieveCalendar(string calName, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return await dataContext.GetCalendar(this.InstanceName, calName, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Get the number of <see cref="T:Quartz.IJob" />s that are
        /// stored in the <see cref="T:Quartz.Spi.IJobStore" />.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public async Task<int> GetNumberOfJobs(CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return await dataContext.Query<Model.IJobDetail>()
                           .Where(j => j.InstanceName == this.InstanceName)
                           .CountAsync(cancellationToken)
                           .PreserveThreadContext();
            }
        }

        /// <summary>
        /// Get the number of <see cref="T:Quartz.ITrigger" />s that are
        /// stored in the <see cref="T:Quartz.Spi.IJobStore" />.
        /// </summary>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public async Task<int> GetNumberOfTriggers(CancellationToken cancellationToken = default)
        {
            return (int)await this.triggerRepository.GetCount();
        }

        /// <summary>
        /// Get the number of <see cref="T:Quartz.ICalendar" /> s that are
        /// stored in the <see cref="T:Quartz.Spi.IJobStore" />.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNumberOfCalendars(CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return await dataContext.Query<Model.ICalendar>().CountAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Get the names of all of the <see cref="T:Quartz.IJob" /> s that
        /// have the given group name.
        /// <para>
        /// If there are no jobs in the given group name, the result should be a
        /// zero-length array (not <see langword="null" />).
        /// </para>
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<JobKey>> GetJobKeys(GroupMatcher<JobKey> matcher, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return (IReadOnlyCollection<JobKey>)new HashSet<JobKey>(
                    await dataContext.GetJobsKeys(this.InstanceName, matcher, cancellationToken).PreserveThreadContext());
            }
        }

        /// <summary>
        /// Get the names of all of the <see cref="T:Quartz.ITrigger" />s
        /// that have the given group name.
        /// <para>
        /// If there are no triggers in the given group name, the result should be a
        /// zero-length array (not <see langword="null" />).
        /// </para>
        /// </summary>
        public async Task<IReadOnlyCollection<TriggerKey>> GetTriggerKeys(GroupMatcher<TriggerKey> matcher, CancellationToken cancellationToken = default)
        {
            return (IReadOnlyCollection<TriggerKey>)new HashSet<TriggerKey>(await this.triggerRepository
                                                                                 .GetTriggerKeys(matcher, cancellationToken).PreserveThreadContext());
        }

        /// <summary>
        /// Get the names of all of the <see cref="T:Quartz.IJob" />
        /// groups.
        /// <para>
        /// If there are no known group names, the result should be a zero-length
        /// array (not <see langword="null" />).
        /// </para>
        /// </summary>
        public async Task<IReadOnlyCollection<string>> GetJobGroupNames(CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return (IReadOnlyCollection<string>)await dataContext.GetJobGroupNames(this.InstanceName, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Get the names of all of the <see cref="T:Quartz.ITrigger" />
        /// groups.
        /// <para>
        /// If there are no known group names, the result should be a zero-length
        /// array (not <see langword="null" />).
        /// </para>
        /// </summary>
        public async Task<IReadOnlyCollection<string>> GetTriggerGroupNames(CancellationToken cancellationToken = default)
        {
            return await this.triggerRepository.GetTriggerGroupNames();
        }

        /// <summary>
        /// Get the names of all of the <see cref="T:Quartz.ICalendar" /> s
        /// in the <see cref="T:Quartz.Spi.IJobStore" />.
        /// <para>
        /// If there are no Calendars in the given group name, the result should be
        /// a zero-length array (not <see langword="null" />).
        /// </para>
        /// </summary>
        public async Task<IReadOnlyCollection<string>> GetCalendarNames(CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return (IReadOnlyCollection<string>)await dataContext.GetCalendarNames(this.InstanceName, cancellationToken);
            }
        }

        /// <summary>
        /// Get all of the Triggers that are associated to the given Job.
        /// </summary>
        /// <remarks>
        /// If there are no matches, a zero-length array should be returned.
        /// </remarks>
        public async Task<IReadOnlyCollection<IOperableTrigger>> GetTriggersForJob(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            var result = await this.triggerRepository.GetTriggers(jobKey, cancellationToken).PreserveThreadContext();
            return result.Select(trigger => trigger.GetTrigger())
                .Cast<IOperableTrigger>()
                .ToList();
        }

        /// <summary>
        /// Get the current state of the identified <see cref="T:Quartz.ITrigger" />.
        /// </summary>
        /// <seealso cref="T:Quartz.TriggerState" />
        public async Task<global::Quartz.TriggerState> GetTriggerState(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            var trigger = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();

            if (trigger == null)
            {
                return TriggerState.None;
            }

            switch (trigger.State)
            {
                case Model.TriggerState.Deleted:
                    return TriggerState.None;
                case Model.TriggerState.Complete:
                    return TriggerState.Complete;
                case Model.TriggerState.Paused:
                case Model.TriggerState.PausedBlocked:
                    return TriggerState.Paused;
                case Model.TriggerState.Error:
                    return TriggerState.Error;
                case Model.TriggerState.Blocked:
                    return TriggerState.Blocked;
                default:
                    return TriggerState.Normal;
            }
        }

        /// <summary>
        /// Pause the <see cref="T:Quartz.ITrigger" /> with the given key.
        /// </summary>
        public async Task PauseTrigger(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.PauseTriggerInternal(triggerKey, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Pause all of the <see cref="T:Quartz.ITrigger" />s in the
        /// given group.
        /// </summary>
        /// <remarks>
        /// The JobStore should "remember" that the group is paused, and impose the
        /// pause on any new triggers that are added to the group while the group is
        /// paused.
        /// </remarks>
        public async Task<IReadOnlyCollection<string>> PauseTriggers(GroupMatcher<TriggerKey> matcher, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.PauseTriggerGroupInternal(dataContext, matcher, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Pause the <see cref="T:Quartz.IJob" /> with the given key - by
        /// pausing all of its current <see cref="T:Quartz.ITrigger" />s.
        /// </summary>
        public async Task PauseJob(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    var triggers = await GetTriggersForJob(jobKey, cancellationToken);
                    foreach (var operableTrigger in triggers)
                    {
                        await this.PauseTriggerInternal(operableTrigger.Key, cancellationToken).PreserveThreadContext();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Pause all of the <see cref="T:Quartz.IJob" />s in the given
        /// group - by pausing all of their <see cref="T:Quartz.ITrigger" />s.
        /// <para>
        /// The JobStore should "remember" that the group is paused, and impose the
        /// pause on any new jobs that are added to the group while the group is
        /// paused.
        /// </para>
        /// </summary>
        /// <seealso cref="T:System.String">
        /// </seealso>
        public async Task<IReadOnlyCollection<string>> PauseJobs(GroupMatcher<JobKey> matcher, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    var jobKeys = await dataContext.GetJobsKeys(this.InstanceName, matcher, cancellationToken).PreserveThreadContext();
                    foreach (var jobKey in jobKeys)
                    {
                        var triggers = await this.triggerRepository.GetTriggers(jobKey, cancellationToken).PreserveThreadContext();
                        foreach (var trigger in triggers)
                        {
                            await this.PauseTriggerInternal(trigger.GetTrigger().Key, cancellationToken).PreserveThreadContext();
                        }
                    }

                    return jobKeys.Select(key => key.Group).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Resume (un-pause) the <see cref="T:Quartz.ITrigger" /> with the
        /// given key.
        /// <para>
        /// If the <see cref="T:Quartz.ITrigger" /> missed one or more fire-times, then the
        /// <see cref="T:Quartz.ITrigger" />'s misfire instruction will be applied.
        /// </para>
        /// </summary>
        /// <seealso cref="T:System.String">
        /// </seealso>
        public async Task ResumeTrigger(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.ResumeTriggerInternal(dataContext, triggerKey, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Resume (un-pause) all of the <see cref="T:Quartz.ITrigger" />s
        /// in the given group.
        /// <para>
        /// If any <see cref="T:Quartz.ITrigger" /> missed one or more fire-times, then the
        /// <see cref="T:Quartz.ITrigger" />'s misfire instruction will be applied.
        /// </para>
        /// </summary>
        public async Task<IReadOnlyCollection<string>> ResumeTriggers(GroupMatcher<TriggerKey> matcher, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.ResumeTriggersInternal(dataContext, matcher, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>Gets the paused trigger groups.</summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<string>> GetPausedTriggerGroups(CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.DataContextFactory(null))
            {
                return (IReadOnlyCollection<string>)new HashSet<string>(
                    await dataContext.GetPausedTriggerGroups(this.InstanceName, cancellationToken: cancellationToken)
                        .PreserveThreadContext());
            }
        }

        /// <summary>
        /// Resume (un-pause) the <see cref="T:Quartz.IJob" /> with the
        /// given key.
        /// <para>
        /// If any of the <see cref="T:Quartz.IJob" />'s<see cref="T:Quartz.ITrigger" /> s missed one
        /// or more fire-times, then the <see cref="T:Quartz.ITrigger" />'s misfire
        /// instruction will be applied.
        /// </para>
        /// </summary>
        public async Task ResumeJob(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    var triggers = await this.triggerRepository.GetTriggers(jobKey, cancellationToken).PreserveThreadContext();
                    await Task.WhenAll(triggers.Select(trigger =>
                        this.ResumeTriggerInternal(dataContext, trigger.GetTrigger().Key, cancellationToken))).PreserveThreadContext();
                }
            }
            catch (AggregateException ex)
            {
                throw new JobPersistenceException(ex.InnerExceptions[0].Message, ex.InnerExceptions[0]);
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Resume (un-pause) all of the <see cref="T:Quartz.IJob" />s in
        /// the given group.
        /// <para>
        /// If any of the <see cref="T:Quartz.IJob" /> s had <see cref="T:Quartz.ITrigger" /> s that
        /// missed one or more fire-times, then the <see cref="T:Quartz.ITrigger" />'s
        /// misfire instruction will be applied.
        /// </para>
        /// </summary>
        public async Task<IReadOnlyCollection<string>> ResumeJobs(GroupMatcher<JobKey> matcher, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    var jobKeys = await dataContext.GetJobsKeys(this.InstanceName, matcher, cancellationToken).PreserveThreadContext();
                    foreach (var jobKey in jobKeys)
                    {
                        var triggers = await this.triggerRepository.GetTriggers(jobKey, cancellationToken).PreserveThreadContext();
                        await Task.WhenAll(triggers.Select(trigger =>
                            this.ResumeTriggerInternal(dataContext, trigger.GetTrigger().Key, cancellationToken))).PreserveThreadContext();
                    }

                    return (IReadOnlyCollection<string>)new HashSet<string>(jobKeys.Select(key => key.Group));
                }
            }
            catch (AggregateException ex)
            {
                throw new JobPersistenceException(ex.InnerExceptions[0].Message, ex.InnerExceptions[0]);
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Pause all triggers - equivalent of calling <see cref="M:Quartz.Spi.IJobStore.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey},System.Threading.CancellationToken)" />
        /// on every group.
        /// <para>
        /// When <see cref="M:Quartz.Spi.IJobStore.ResumeAll(System.Threading.CancellationToken)" /> is called (to un-pause), trigger misfire
        /// instructions WILL be applied.
        /// </para>
        /// </summary>
        /// <seealso cref="M:Quartz.Spi.IJobStore.ResumeAll(System.Threading.CancellationToken)" />
        public async Task PauseAll(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.PauseAllInternal(dataContext, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Resume (un-pause) all triggers - equivalent of calling <see cref="M:Quartz.Spi.IJobStore.ResumeTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey},System.Threading.CancellationToken)" />
        /// on every group.
        /// <para>
        /// If any <see cref="T:Quartz.ITrigger" /> missed one or more fire-times, then the
        /// <see cref="T:Quartz.ITrigger" />'s misfire instruction will be applied.
        /// </para>
        /// </summary>
        /// <seealso cref="M:Quartz.Spi.IJobStore.PauseAll(System.Threading.CancellationToken)" />
        public async Task ResumeAll(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.ResumeAllInternal(dataContext, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Get a handle to the next trigger to be fired, and mark it as 'reserved'
        /// by the calling scheduler.
        /// </summary>
        /// <param name="noLaterThan">If &gt; 0, the JobStore should only return a Trigger
        /// that will fire no later than the time represented in this value as
        /// milliseconds.</param>
        /// <param name="maxCount"></param>
        /// <param name="timeWindow"></param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        /// <seealso cref="T:Quartz.ITrigger">
        /// </seealso>
        public async Task<IReadOnlyCollection<IOperableTrigger>> AcquireNextTriggers(
            DateTimeOffset noLaterThan,
            int maxCount,
            TimeSpan timeWindow,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    return await this.AcquireNextTriggersInternal(dataContext, noLaterThan, maxCount, timeWindow, cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Inform the <see cref="T:Quartz.Spi.IJobStore" /> that the scheduler no longer plans to
        /// fire the given <see cref="T:Quartz.ITrigger" />, that it had previously acquired
        /// (reserved).
        /// </summary>
        public async Task ReleaseAcquiredTrigger(IOperableTrigger trigger, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.triggerRepository.UpdateTriggerState(trigger.Key, Model.TriggerState.Waiting,
                        Model.TriggerState.Acquired);
                    await dataContext.DeleteFiredTrigger(this.InstanceName, trigger.FireInstanceId, cancellationToken: cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Inform the <see cref="T:Quartz.Spi.IJobStore" /> that the scheduler is now firing the
        /// given <see cref="T:Quartz.ITrigger" /> (executing its associated <see cref="T:Quartz.IJob" />),
        /// that it had previously acquired (reserved).
        /// </summary>
        /// <returns>
        /// May return null if all the triggers or their calendars no longer exist, or
        /// if the trigger was not successfully put into the 'executing'
        /// state.  Preference is to return an empty list if none of the triggers
        /// could be fired.
        /// </returns>
        public async Task<IReadOnlyCollection<TriggerFiredResult>> TriggersFired(IReadOnlyCollection<IOperableTrigger> triggers, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken))
                {
                    var results = new List<TriggerFiredResult>();

                    foreach (var operableTrigger in triggers)
                    {
                        TriggerFiredResult result;
                        try
                        {
                            var bundle = await this.TriggerFiredInternal(dataContext, operableTrigger, cancellationToken);
                            result = new TriggerFiredResult(bundle);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error($"Caught exception: {ex.Message}", ex);
                            result = new TriggerFiredResult(ex);
                        }

                        results.Add(result);
                    }

                    return results;
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Inform the <see cref="T:Quartz.Spi.IJobStore" /> that the scheduler has completed the
        /// firing of the given <see cref="T:Quartz.ITrigger" /> (and the execution its
        /// associated <see cref="T:Quartz.IJob" />), and that the <see cref="T:Quartz.JobDataMap" />
        /// in the given <see cref="T:Quartz.IJobDetail" /> should be updated if the <see cref="T:Quartz.IJob" />
        /// is stateful.
        /// </summary>
        public async Task TriggeredJobComplete(
            IOperableTrigger trigger,
            global::Quartz.IJobDetail jobDetail,
            SchedulerInstruction triggerInstCode,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var dataContext = this.DataContextFactory(null))
                using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                {
                    await this.TriggeredJobCompleteInternal(dataContext, trigger, jobDetail, triggerInstCode, cancellationToken).PreserveThreadContext();
                }

                var sigTime = this.ClearAndGetSignalSchedulingChangeOnTxCompletion();
                if (sigTime != null)
                {
                    this.SignalSchedulingChangeImmediately(sigTime);
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        internal async Task<RecoverMisfiredJobsResult> DoRecoverMisfires(CancellationToken cancellationToken)
        {
            try
            {
                var result = RecoverMisfiredJobsResult.NoOp;

                using (var dataContext = this.DataContextFactory(null))
                {
                    var misfireCount = await this.triggerRepository.GetMisfireCount(MisfireTime.UtcDateTime);
                    if (misfireCount == 0)
                    {
                        this.Logger.Debug("Found 0 triggers that missed their scheduled fire-time.");
                    }
                    else
                    {
                        using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
                        {
                            result = await this.RecoverMisfiredJobsInternal(dataContext, false, cancellationToken).PreserveThreadContext();
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        private async Task RecoverJobs(IDataContext dataContext, CancellationToken cancellationToken)
        {
            using (await this.lockManager.AcquireLock(dataContext, LockType.TriggerAccess, this.InstanceId, cancellationToken).PreserveThreadContext())
            {
                await this.RecoverJobsInternal(dataContext, cancellationToken).PreserveThreadContext();
            }
        }

        private async Task PauseTriggerInternal(TriggerKey triggerKey, CancellationToken cancellationToken)
        {
            var trigger = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();
            switch (trigger.State)
            {
                case Model.TriggerState.Waiting:
                case Model.TriggerState.Acquired:
                    await this.triggerRepository.UpdateTriggerState(triggerKey, Model.TriggerState.Paused)
                        ;
                    break;
                case Model.TriggerState.Blocked:
                    await this.triggerRepository.UpdateTriggerState(triggerKey, Model.TriggerState.PausedBlocked)
                        ;
                    break;
            }
        }

        private async Task<IReadOnlyCollection<string>> PauseTriggerGroupInternal(
            IDataContext dataContext,
            GroupMatcher<TriggerKey> matcher,
            CancellationToken token = default)
        {
            await this.triggerRepository.UpdateTriggersStates(matcher, Model.TriggerState.Paused,
                Model.TriggerState.Acquired,
                Model.TriggerState.Waiting);
            await this.triggerRepository.UpdateTriggersStates(matcher, Model.TriggerState.PausedBlocked,
                Model.TriggerState.Blocked);

            var triggerGroups = await this.triggerRepository.GetTriggerGroupNames(matcher);

            // make sure to account for an exact group match for a group that doesn't yet exist
            var op = matcher.CompareWithOperator;
            if (op.Equals(StringOperator.Equality) && !triggerGroups.Contains(matcher.CompareToValue))
            {
                triggerGroups.Add(matcher.CompareToValue);
            }

            foreach (var triggerGroup in triggerGroups)
                if (!await dataContext.IsTriggerGroupPaused(this.InstanceName, triggerGroup, cancellationToken: token).PreserveThreadContext())
                {
                    await dataContext.AddPausedTriggerGroup(this.InstanceName, triggerGroup, cancellationToken: token).PreserveThreadContext();
                }

            return (IReadOnlyCollection<string>)new HashSet<string>(triggerGroups);
        }

        private async Task PauseAllInternal(IDataContext dataContext, CancellationToken cancellationToken = default)
        {
            var groupNames = await this.triggerRepository.GetTriggerGroupNames();

            await Task.WhenAll(groupNames.Select(groupName =>
                this.PauseTriggerGroupInternal(dataContext, GroupMatcher<TriggerKey>.GroupEquals(groupName), cancellationToken))).PreserveThreadContext();

            if (!await dataContext.IsTriggerGroupPaused(this.InstanceName, AllGroupsPaused, cancellationToken: cancellationToken).PreserveThreadContext())
            {
                await dataContext.AddPausedTriggerGroup(this.InstanceName, AllGroupsPaused, cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        private async Task<bool> ReplaceTriggerInternal(
            IDataContext dataContext,
            TriggerKey triggerKey,
            IOperableTrigger newTrigger,
            CancellationToken cancellationToken)
        {
            var trigger = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();
            var job = await dataContext.GetJobDetail(this.InstanceName, trigger.GetJobKey(), cancellationToken).PreserveThreadContext();

            if (job == null)
            {
                return false;
            }

            if (!newTrigger.JobKey.Equals(job.Key))
            {
                throw new JobPersistenceException("New trigger is not related to the same job as the old trigger.");
            }

            var removedTrigger = await this.triggerRepository.DeleteTrigger(triggerKey);
            await this.StoreTriggerInternal(dataContext, newTrigger, job, false, Model.TriggerState.Waiting, false, false, cancellationToken).PreserveThreadContext();
            return removedTrigger > 0;
        }

        private async Task<bool> RemoveJobInternal(
            IDataContext dataContext,
            JobKey jobKey,
            CancellationToken cancellationToken)
        {
            await this.triggerRepository.DeleteTriggers(jobKey);
            var result = await dataContext.DeleteJob(this.InstanceName, jobKey, cancellationToken).PreserveThreadContext();
            return result > 0;
        }

        private async Task<bool> RemoveTriggerInternal(
            IDataContext dataContext,
            TriggerKey key,
            global::Quartz.IJobDetail job = null,
            CancellationToken cancellationToken = default)
        {
            var trigger = await this.triggerRepository.GetTrigger(key, cancellationToken).PreserveThreadContext();
            if (trigger == null)
            {
                return false;
            }

            if (job == null)
            {
                job = await dataContext.GetJobDetail(this.InstanceName, trigger.GetJobKey(), cancellationToken).PreserveThreadContext();
            }

            var removedTrigger = await this.triggerRepository.DeleteTrigger(key) > 0;

            if (job != null && !job.Durable)
            {
                if (await this.triggerRepository.GetCount(job.Key) == 0)
                {
                    if (await this.RemoveJobInternal(dataContext, job.Key, cancellationToken).PreserveThreadContext())
                    {
                        await this.schedulerSignaler.NotifySchedulerListenersJobDeleted(job.Key, cancellationToken).PreserveThreadContext();
                    }
                }
            }

            return removedTrigger;
        }

        private async Task<bool> RemoveCalendarInternal(IDataContext dataContext, string calendarName, CancellationToken cancellationToken)
        {
            if (await this.triggerRepository.TriggersExists(calendarName, cancellationToken).PreserveThreadContext())
            {
                throw new JobPersistenceException("Calender cannot be removed if it referenced by a trigger!");
            }

            return await dataContext.DeleteCalendar(this.InstanceName, calendarName, cancellationToken: cancellationToken).PreserveThreadContext() > 0;
        }

        private async Task ResumeTriggerInternal(
            IDataContext dataContext,
            TriggerKey triggerKey,
            CancellationToken cancellationToken = default)
        {
            var trigger = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();
            if (trigger?.NextFireTime == null || trigger.NextFireTime == DateTime.MinValue)
            {
                return;
            }

            var blocked = trigger.State == Model.TriggerState.PausedBlocked;
            var newState = await this.CheckBlockedState(dataContext, trigger.GetJobKey(), Model.TriggerState.Waiting, cancellationToken).PreserveThreadContext();
            var misfired = false;

            if (this.schedulerRunning && trigger.NextFireTime < DateTime.UtcNow)
            {
                misfired = await UpdateMisfiredTrigger(dataContext, triggerKey, newState, true, cancellationToken).PreserveThreadContext();
            }

            if (!misfired)
            {
                await this.triggerRepository.UpdateTriggerState(triggerKey, newState,
                    blocked ? Model.TriggerState.PausedBlocked : Model.TriggerState.Paused);
            }
        }

        private async Task<IReadOnlyCollection<string>> ResumeTriggersInternal(
            IDataContext dataContext,
            GroupMatcher<TriggerKey> matcher,
            CancellationToken token = default)
        {
            await dataContext.DeletePausedTriggerGroup(this.InstanceName, matcher, cancellationToken: token).PreserveThreadContext();
            var groups = new HashSet<string>();

            var keys = await this.triggerRepository.GetTriggerKeys(matcher, token).PreserveThreadContext();
            foreach (var triggerKey in keys)
            {
                await this.ResumeTriggerInternal(dataContext, triggerKey, token).PreserveThreadContext();
                groups.Add(triggerKey.Group);
            }

            return groups.ToList();
        }

        private async Task ResumeAllInternal(IDataContext dataContext, CancellationToken cancellationToken = default)
        {
            var groupNames = await this.triggerRepository.GetTriggerGroupNames();
            await Task.WhenAll(groupNames.Select(groupName =>
                this.ResumeTriggersInternal(dataContext, GroupMatcher<TriggerKey>.GroupEquals(groupName), cancellationToken))).PreserveThreadContext();
            await dataContext.DeletePausedTriggerGroup(this.InstanceName, AllGroupsPaused, cancellationToken: cancellationToken).PreserveThreadContext();
        }

        private async Task StoreCalendarInternal(
            IDataContext dataContext, string calName, global::Quartz.ICalendar calendar, bool replaceExisting,
            bool updateTriggers, CancellationToken token = default)
        {
            var existingCal = await dataContext.CalendarExists(this.InstanceName, calName, cancellationToken: token).PreserveThreadContext();
            if (existingCal && !replaceExisting)
            {
                throw new ObjectAlreadyExistsException("Calendar with name '" + calName + "' already exists.");
            }

            if (existingCal)
            {
                if (await dataContext.UpdateCalendar(this.InstanceName, calName, calendar, cancellationToken: token) == 0)
                {
                    throw new JobPersistenceException("Couldn't store calendar.  Update failed.");
                }

                if (updateTriggers)
                {
                    var triggers = await this.triggerRepository.GetTriggers(calName, token).PreserveThreadContext();
                    foreach (var trigger in triggers)
                    {
                        var quartzTrigger = (IOperableTrigger)trigger.GetTrigger();
                        quartzTrigger.UpdateWithNewCalendar(calendar, this.MisfireThreshold);
                        await this.StoreTriggerInternal(
                            dataContext,
                            quartzTrigger, null, true, Model.TriggerState.Waiting, false, false,
                            token).PreserveThreadContext();
                    }
                }
            }
            else
            {
                await dataContext.AddCalendar(calName, calendar, cancellationToken: token).PreserveThreadContext();
            }
        }

        private async Task StoreJobInternal(
            IDataContext dataContext,
            global::Quartz.IJobDetail newJob,
            bool replaceExisting,
            CancellationToken cancellationToken)
        {
            var existingJob = await dataContext.JobExists(this.InstanceName, newJob.Key, cancellationToken: cancellationToken).PreserveThreadContext();

            if (existingJob)
            {
                if (!replaceExisting)
                {
                    throw new ObjectAlreadyExistsException(newJob);
                }

                await dataContext.UpdateJob(this.InstanceName, newJob, true, cancellationToken: cancellationToken).PreserveThreadContext();
            }
            else
            {
                await dataContext.AddJob(this.InstanceName, newJob, cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        private async Task StoreTriggerInternal(
            IDataContext dataContext,
            IOperableTrigger newTrigger,
            global::Quartz.IJobDetail job,
            bool replaceExisting,
            Model.TriggerState state,
            bool forceState,
            bool recovering,
            CancellationToken token = default)
        {
            var existingTrigger = await this.triggerRepository.TriggerExists(newTrigger.Key, token).PreserveThreadContext();

            if (existingTrigger && !replaceExisting)
            {
                throw new ObjectAlreadyExistsException(newTrigger);
            }

            if (!forceState)
            {
                var shouldBePaused = await dataContext.IsTriggerGroupPaused(this.InstanceName, newTrigger.Key.Group, token).PreserveThreadContext();

                if (!shouldBePaused)
                {
                    shouldBePaused = await dataContext.IsTriggerGroupPaused(this.InstanceName, AllGroupsPaused, cancellationToken: token).PreserveThreadContext();
                    if (shouldBePaused)
                    {
                        await dataContext.AddPausedTriggerGroup(this.InstanceName, newTrigger.Key.Group, cancellationToken: token).PreserveThreadContext();
                    }
                }

                if (shouldBePaused && state == Model.TriggerState.Waiting || state == Model.TriggerState.Acquired)
                {
                    state = Model.TriggerState.Paused;
                }
            }

            if (job == null)
            {
                job = await dataContext.GetJobDetail(this.InstanceName, newTrigger.JobKey, token).PreserveThreadContext();
            }

            if (job == null)
            {
                throw new JobPersistenceException(
                    $"The job ({newTrigger.JobKey}) referenced by the trigger does not exist.");
            }

            if (job.ConcurrentExecutionDisallowed && !recovering)
            {
                state = await CheckBlockedState(dataContext, job.Key, state, token).PreserveThreadContext();
            }

            if (existingTrigger)
            {
                await this.triggerRepository.UpdateTrigger(this.triggerFactory.CreateTrigger(newTrigger, state, this.InstanceName));
            }
            else
            {
                await this.triggerRepository.AddTrigger(this.triggerFactory.CreateTrigger(newTrigger, state, this.InstanceName));
            }
        }

        private async Task<Model.TriggerState> CheckBlockedState(
            IDataContext dataContext,
            JobKey jobKey,
            Model.TriggerState currentState,
            CancellationToken cancellationToken)
        {
            if (currentState != Model.TriggerState.Waiting && currentState != Model.TriggerState.Paused)
            {
                return currentState;
            }

            var firedTrigger = (await dataContext.GetFiredTriggers(this.InstanceName, jobKey, cancellationToken: cancellationToken).PreserveThreadContext())
                .FirstOrDefault();
            if (firedTrigger != null)
            {
                if (firedTrigger.ConcurrentExecutionDisallowed)
                {
                    return currentState == Model.TriggerState.Paused
                        ? Model.TriggerState.PausedBlocked
                        : Model.TriggerState.Blocked;
                }
            }

            return currentState;
        }

        private async Task<TriggerFiredBundle> TriggerFiredInternal(IDataContext dataContext, IOperableTrigger trigger, CancellationToken cancellationToken = default)
        {
            var state = await dataContext.GetTriggerState(this.InstanceName, trigger.Key, cancellationToken).PreserveThreadContext();
            if (state != Model.TriggerState.Acquired)
            {
                return null;
            }

            var job = await dataContext.GetJob(this.InstanceName, trigger.JobKey, cancellationToken).PreserveThreadContext();
            if (job == null)
            {
                return null;
            }

            global::Quartz.ICalendar calendar = null;
            if (trigger.CalendarName != null)
            {
                calendar = await dataContext.GetCalendar(
                               this.InstanceName,
                               trigger.CalendarName,
                               cancellationToken: cancellationToken).PreserveThreadContext();
                if (calendar == null)
                {
                    return null;
                }
            }

            await dataContext.UpdateFiredTrigger(
                this.InstanceName,
                this.InstanceId,
                trigger.FireInstanceId,
                this.triggerFactory.CreateTrigger(trigger, Model.TriggerState.Executing, this.InstanceName),
                job,
                cancellationToken).PreserveThreadContext();

            var prevFireTime = trigger.GetPreviousFireTimeUtc();
            trigger.Triggered(calendar);

            state = Model.TriggerState.Waiting;
            var force = true;

            if (job.ConcurrentExecutionDisallowed)
            {
                state = Model.TriggerState.Blocked;
                force = false;
                await this.triggerRepository.UpdateTriggersStates(trigger.JobKey, Model.TriggerState.Blocked,
                    Model.TriggerState.Waiting);
                await this.triggerRepository.UpdateTriggersStates(trigger.JobKey, Model.TriggerState.Blocked,
                    Model.TriggerState.Acquired);
                await this.triggerRepository.UpdateTriggersStates(trigger.JobKey, Model.TriggerState.PausedBlocked,
                    Model.TriggerState.Paused);
            }

            if (!trigger.GetNextFireTimeUtc().HasValue)
            {
                state = Model.TriggerState.Complete;
                force = true;
            }

            var jobDetail = job.GetJobDetail();
            await this.StoreTriggerInternal(dataContext, trigger, jobDetail, true, state, force, force).PreserveThreadContext();

            jobDetail.JobDataMap.ClearDirtyFlag();

            return new TriggerFiredBundle(jobDetail,
                trigger,
                calendar,
                trigger.Key.Group.Equals(SchedulerConstants.DefaultRecoveryGroup),
                DateTimeOffset.UtcNow,
                trigger.GetPreviousFireTimeUtc(),
                prevFireTime,
                trigger.GetNextFireTimeUtc());
        }

        private async Task<bool> UpdateMisfiredTrigger(
            IDataContext dataContext,
            TriggerKey triggerKey,
            Model.TriggerState newStateIfNotComplete,
            bool forceState,
            CancellationToken cancellationToken = default)
        {
            var trigger = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();
            var misfireTime = DateTime.Now;
            if (MisfireThreshold > TimeSpan.Zero)
            {
                misfireTime = misfireTime.AddMilliseconds(-1 * MisfireThreshold.TotalMilliseconds);
            }

            if (trigger.NextFireTime > misfireTime)
            {
                return false;
            }

            await this.DoUpdateOfMisfiredTrigger(
                dataContext,
                trigger,
                forceState,
                newStateIfNotComplete,
                false,
                cancellationToken).PreserveThreadContext();

            return true;
        }

        private async Task DoUpdateOfMisfiredTrigger(
            IDataContext dataContext,
            Model.ITrigger trigger, bool forceState,
            Model.TriggerState newStateIfNotComplete, bool recovering,
            CancellationToken cancellationToken = default)
        {
            var operableTrigger = (IOperableTrigger)trigger.GetTrigger();

            global::Quartz.ICalendar cal = null;
            if (trigger.CalendarName != null)
            {
                cal = await dataContext.GetCalendar(this.InstanceName, trigger.CalendarName, cancellationToken).PreserveThreadContext();
            }

            await this.schedulerSignaler.NotifyTriggerListenersMisfired(operableTrigger, cancellationToken).PreserveThreadContext();
            operableTrigger.UpdateAfterMisfire(cal);

            if (!operableTrigger.GetNextFireTimeUtc().HasValue)
            {
                await this.StoreTriggerInternal(
                    dataContext,
                    operableTrigger,
                    null,
                    true,
                    Model.TriggerState.Complete,
                    forceState,
                    recovering,
                    cancellationToken).PreserveThreadContext();
                await this.schedulerSignaler.NotifySchedulerListenersFinalized(operableTrigger, cancellationToken).PreserveThreadContext();
            }
            else
            {
                await this.StoreTriggerInternal(
                    dataContext,
                    operableTrigger,
                    null,
                    true,
                    newStateIfNotComplete,
                    forceState,
                    false,
                    cancellationToken).PreserveThreadContext();
            }
        }

        private async Task<IReadOnlyCollection<IOperableTrigger>> AcquireNextTriggersInternal(
            IDataContext dataContext,
            DateTimeOffset noLaterThan,
            int maxCount,
            TimeSpan timeWindow,
            CancellationToken cancellationToken)
        {
            if (timeWindow < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeWindow));
            }

            var acquiredTriggers = new List<IOperableTrigger>();
            var acquiredJobKeysForNoConcurrentExec = new HashSet<JobKey>();

            const int maxDoLoopRetry = 3;
            var currentLoopCount = 0;

            do
            {
                currentLoopCount++;
                var keys = await this.triggerRepository
                    .GetTriggersToAcquire(noLaterThan + timeWindow, MisfireTime, maxCount);

                if (!keys.Any())
                {
                    return acquiredTriggers;
                }

                foreach (var triggerKey in keys)
                {
                    var nextTrigger = await this.triggerRepository.GetTrigger(triggerKey, cancellationToken).PreserveThreadContext();
                    if (nextTrigger == null)
                    {
                        continue;
                    }

                    var jobKey = nextTrigger.GetJobKey();
                    Model.IJobDetail jobDetail;
                    try
                    {
                        jobDetail = await dataContext.GetJob(this.InstanceName, jobKey, cancellationToken).PreserveThreadContext();
                    }
                    catch (Exception)
                    {
                        await this.triggerRepository.UpdateTriggerState(triggerKey, Model.TriggerState.Error)
                            ;
                        continue;
                    }

                    if (jobDetail.ConcurrentExecutionDisallowed)
                    {
                        if (acquiredJobKeysForNoConcurrentExec.Contains(jobKey))
                        {
                            continue;
                        }

                        acquiredJobKeysForNoConcurrentExec.Add(jobKey);
                    }

                    var result = await this.triggerRepository.UpdateTriggerState(triggerKey, Model.TriggerState.Acquired,
                        Model.TriggerState.Waiting);
                    if (result <= 0)
                    {
                        continue;
                    }

                    var operableTrigger = (IOperableTrigger)nextTrigger.GetTrigger();
                    operableTrigger.FireInstanceId = this.GetFiredTriggerRecordId();

                    await dataContext.AddFiredTrigger(
                        operableTrigger.FireInstanceId,
                        this.InstanceId,
                        nextTrigger,
                        null,
                        cancellationToken).PreserveThreadContext();

                    acquiredTriggers.Add(operableTrigger);
                }

                if (acquiredTriggers.Count == 0 && currentLoopCount < maxDoLoopRetry)
                {
                    continue;
                }

                break;
            } while (true);

            return acquiredTriggers;
        }

        private string GetFiredTriggerRecordId()
        {
            Interlocked.Increment(ref fireTriggerRecordCounter);
            return InstanceId + fireTriggerRecordCounter;
        }

        private async Task TriggeredJobCompleteInternal(
            IDataContext dataContext,
            IOperableTrigger trigger,
            global::Quartz.IJobDetail jobDetail,
            SchedulerInstruction triggerInstCode,
            CancellationToken token = default)
        {
            try
            {
                switch (triggerInstCode)
                {
                    case SchedulerInstruction.DeleteTrigger:
                        if (!trigger.GetNextFireTimeUtc().HasValue)
                        {
                            var trig = await this.triggerRepository.GetTrigger(trigger.Key, token).PreserveThreadContext();
                            if (trig != null && !trig.NextFireTime.HasValue)
                            {
                                await this.RemoveTriggerInternal(dataContext, trigger.Key, jobDetail, token).PreserveThreadContext();
                            }
                        }
                        else
                        {
                            await this.RemoveTriggerInternal(dataContext, trigger.Key, jobDetail, token).PreserveThreadContext();
                            SignalSchedulingChangeOnTxCompletion(SchedulingSignalDateTime);
                        }

                        break;
                    case SchedulerInstruction.SetTriggerComplete:
                        await this.triggerRepository.UpdateTriggerState(trigger.Key, Model.TriggerState.Complete)
                            ;
                        SignalSchedulingChangeOnTxCompletion(SchedulingSignalDateTime);
                        break;
                    case SchedulerInstruction.SetTriggerError:
                        this.Logger.Info("Trigger " + trigger.Key + " set to ERROR state.");
                        await this.triggerRepository.UpdateTriggerState(trigger.Key, Model.TriggerState.Error)
                            ;
                        SignalSchedulingChangeOnTxCompletion(SchedulingSignalDateTime);
                        break;
                    case SchedulerInstruction.SetAllJobTriggersComplete:
                        await this.triggerRepository.UpdateTriggersStates(trigger.JobKey, Model.TriggerState.Complete)
                            ;
                        SignalSchedulingChangeOnTxCompletion(SchedulingSignalDateTime);
                        break;
                    case SchedulerInstruction.SetAllJobTriggersError:
                        this.Logger.Info("All triggers of Job " + trigger.JobKey + " set to ERROR state.");
                        await this.triggerRepository.UpdateTriggersStates(trigger.JobKey, Model.TriggerState.Error)
                            ;
                        SignalSchedulingChangeOnTxCompletion(SchedulingSignalDateTime);
                        break;
                }

                if (jobDetail.ConcurrentExecutionDisallowed)
                {
                    await this.triggerRepository.UpdateTriggersStates(jobDetail.Key, Model.TriggerState.Waiting,
                        Model.TriggerState.Blocked);
                    await this.triggerRepository.UpdateTriggersStates(jobDetail.Key, Model.TriggerState.Paused,
                        Model.TriggerState.PausedBlocked);
                    SignalSchedulingChangeOnTxCompletion(SchedulingSignalDateTime);
                }

                if (jobDetail.PersistJobDataAfterExecution && jobDetail.JobDataMap.Dirty)
                {
                    await dataContext.UpdateJobData(this.InstanceName, jobDetail.Key, jobDetail.JobDataMap, cancellationToken: token).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }

            try
            {
                await dataContext.DeleteFiredTrigger(this.InstanceName, trigger.FireInstanceId, cancellationToken: token).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                throw new JobPersistenceException(ex.Message, ex);
            }
        }

        protected virtual void SignalSchedulingChangeOnTxCompletion(DateTimeOffset? candidateNewNextFireTime)
        {
            var sigTime = LogicalThreadContext.GetData<DateTimeOffset?>(KeySignalChangeForTxCompletion);
            if (sigTime == null && candidateNewNextFireTime.HasValue)
            {
                LogicalThreadContext.SetData(KeySignalChangeForTxCompletion, candidateNewNextFireTime);
            }
            else
            {
                if (sigTime == null || candidateNewNextFireTime < sigTime)
                {
                    LogicalThreadContext.SetData(KeySignalChangeForTxCompletion, candidateNewNextFireTime);
                }
            }
        }

        protected virtual DateTimeOffset? ClearAndGetSignalSchedulingChangeOnTxCompletion()
        {
            var t = LogicalThreadContext.GetData<DateTimeOffset?>(KeySignalChangeForTxCompletion);
            LogicalThreadContext.FreeNamedDataSlot(KeySignalChangeForTxCompletion);
            return t;
        }

        internal virtual void SignalSchedulingChangeImmediately(DateTimeOffset? candidateNewNextFireTime)
        {
            this.schedulerSignaler.SignalSchedulingChange(candidateNewNextFireTime);
        }

        private async Task RecoverJobsInternal(IDataContext dataContext, CancellationToken cancellationToken)
        {
            var result = await this.triggerRepository.UpdateTriggersStates(Model.TriggerState.Waiting,
                Model.TriggerState.Acquired, Model.TriggerState.Blocked);
            result += await this.triggerRepository.UpdateTriggersStates(Model.TriggerState.Paused,
                Model.TriggerState.PausedBlocked);

            this.Logger.Info("Freed " + result + " triggers from 'acquired' / 'blocked' state.");

            await this.RecoverMisfiredJobsInternal(dataContext, true, cancellationToken).PreserveThreadContext();

            var results = (await dataContext.GetRecoverableFiredTriggers(this.InstanceName, this.InstanceId, cancellationToken: cancellationToken).PreserveThreadContext())
                .Select(async trigger =>
                    trigger.GetRecoveryTrigger(await dataContext.GetTriggerJobDataMap(this.InstanceName, trigger.GetTriggerKey(), cancellationToken: cancellationToken).PreserveThreadContext()));
            var recoveringJobTriggers = (await Task.WhenAll(results)).ToList();

            this.Logger.Info("Recovering " + recoveringJobTriggers.Count +
                     " jobs that were in-progress at the time of the last shut-down.");

            foreach (var recoveringJobTrigger in recoveringJobTriggers)
            {
                if (await dataContext.JobExists(this.InstanceName, recoveringJobTrigger.JobKey, cancellationToken: cancellationToken).PreserveThreadContext())
                {
                    recoveringJobTrigger.ComputeFirstFireTimeUtc(null);
                    await this.StoreTriggerInternal(
                        dataContext,
                        recoveringJobTrigger,
                        null,
                        false,
                        Model.TriggerState.Waiting,
                        false,
                        true,
                        cancellationToken).PreserveThreadContext();
                }
            }

            this.Logger.Info("Recovery complete");

            var completedTriggers =
                await this.triggerRepository.GetTriggerKeys(Model.TriggerState.Complete);
            foreach (var completedTrigger in completedTriggers)
            {
                await this.RemoveTriggerInternal(dataContext, completedTrigger, cancellationToken: cancellationToken).PreserveThreadContext();
            }

            this.Logger.Info($"Removed {completedTriggers.Count} 'complete' triggers.");

            result = await dataContext.DeleteFiredTriggersByInstanceId(
                         this.InstanceName,
                         this.InstanceId,
                         cancellationToken: cancellationToken).PreserveThreadContext();
            this.Logger.Info("Removed " + result + " stale fired job entries.");
        }

        private async Task<RecoverMisfiredJobsResult> RecoverMisfiredJobsInternal(IDataContext dataContext, bool recovering, CancellationToken cancellationToken = default)
        {
            var maxMisfiresToHandleAtTime = recovering ? -1 : MaxMisfiresToHandleAtATime;
            var earliestNewTime = DateTime.MaxValue;

            var hasMoreMisfiredTriggers = this.triggerRepository.HasMisfiredTriggers(MisfireTime.UtcDateTime,
                maxMisfiresToHandleAtTime, out var misfiredTriggers);

            if (hasMoreMisfiredTriggers)
            {
                this.Logger.Info(
                    "Handling the first " + misfiredTriggers.Count +
                    " triggers that missed their scheduled fire-time.  " +
                    "More misfired triggers remain to be processed.");
            }
            else if (misfiredTriggers.Count > 0)
            {
                this.Logger.Info(
                    "Handling " + misfiredTriggers.Count +
                    " trigger(s) that missed their scheduled fire-time.");
            }
            else
            {
                this.Logger.Debug(
                    "Found 0 triggers that missed their scheduled fire-time.");
                return RecoverMisfiredJobsResult.NoOp;
            }

            foreach (var misfiredTrigger in misfiredTriggers)
            {
                var trigger = await this.triggerRepository.GetTrigger(misfiredTrigger, cancellationToken).PreserveThreadContext();

                if (trigger == null)
                {
                    continue;
                }

                await this.DoUpdateOfMisfiredTrigger(
                    dataContext,
                    trigger,
                    false,
                    Model.TriggerState.Waiting,
                    recovering,
                    cancellationToken).PreserveThreadContext();

                var nextTime = trigger.NextFireTime;
                if (nextTime.HasValue && nextTime.Value < earliestNewTime)
                {
                    earliestNewTime = nextTime.Value;
                }
            }

            return new RecoverMisfiredJobsResult(hasMoreMisfiredTriggers, misfiredTriggers.Count,
                earliestNewTime);
        }
    }
}