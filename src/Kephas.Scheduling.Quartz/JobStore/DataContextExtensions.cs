// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Quartz;
    using global::Quartz.Impl.Matchers;
    using global::Quartz.Simpl;
    using global::Quartz.Spi;

    using Kephas.Data;
    using Kephas.Data.Linq;
    using Kephas.Logging;
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Scheduling.Quartz.Linq;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A data context extensions.
    /// </summary>
    public static class DataContextExtensions
    {
        /// <summary>
        /// The object serializer.
        /// </summary>
        internal static readonly IObjectSerializer ObjectSerializer = new DefaultObjectSerializer();

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Log = typeof(DataContextExtensions).GetLogger();

        /// <summary>
        /// Adds a calendar.
        /// </summary>
        /// <param name="dataContext">The dataContext to act on.</param>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static async Task AddCalendar(this IDataContext dataContext, string calendarName, global::Quartz.ICalendar calendar, CancellationToken cancellationToken = default)
        {
            var entity = await dataContext.CreateEntityAsync<Model.ICalendar>(cancellationToken: cancellationToken).PreserveThreadContext();
            entity.CalendarName = calendarName;
            entity.Content = ObjectSerializer.Serialize(calendar);
            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Gets the calendar names.
        /// </summary>
        /// <param name="dataContext">The dataContext to act on.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the calendar names.
        /// </returns>
        public static async Task<IEnumerable<string>> GetCalendarNames(this IDataContext dataContext, string instanceName, CancellationToken cancellationToken = default)
        {
            var calendars = await dataContext.Query<Model.ICalendar>().Where(c => c.InstanceName == instanceName).ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            return calendars.Select(c => c.CalendarName).Distinct();
        }

        /// <summary>
        /// Queries if a given calendar exists.
        /// </summary>
        /// <param name="dataContext">The dataContext to act on.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields true if it succeeds, false if it fails.
        /// </returns>
        public static Task<bool> CalendarExists(this IDataContext dataContext, string instanceName, string calendarName, CancellationToken cancellationToken = default)
        {
            return dataContext.Query<Model.ICalendar>()
                .Where(c => c.CalendarName == calendarName && c.InstanceName == instanceName)
                .AnyAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets a calendar by name.
        /// </summary>
        /// <param name="dataContext">The dataContext to act on.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the calendar.
        /// </returns>
        public static async Task<global::Quartz.ICalendar> GetCalendar(this IDataContext dataContext, string instanceName, string calendarName, CancellationToken cancellationToken = default)
        {
            var calendar = await dataContext.FindOneAsync<Model.ICalendar>(
                               c => c.CalendarName == calendarName && c.InstanceName == instanceName,
                               throwIfNotFound: false,
                               cancellationToken: cancellationToken).PreserveThreadContext();
            return calendar?.GetCalendar();
        }

        /// <summary>
        /// Updates the calendar.
        /// </summary>
        /// <param name="dataContext">The dataContext to act on.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the number of updated entries.
        /// </returns>
        public static async Task<long> UpdateCalendar(this IDataContext dataContext, string instanceName, string calendarName, global::Quartz.ICalendar calendar, CancellationToken cancellationToken = default)
        {
            var entity = await dataContext.FindOneAsync<Model.ICalendar>(
                             c => c.InstanceName == instanceName && c.CalendarName == calendarName,
                             cancellationToken: cancellationToken).PreserveThreadContext();
            entity.Content = ObjectSerializer.Serialize(calendar);

            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            return 1;
        }

        /// <summary>
        /// Deletes the calendar.
        /// </summary>
        /// <param name="dataContext">The dataContext to act on.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the number of deleted entries.
        /// </returns>
        public static Task<long> DeleteCalendar(this IDataContext dataContext, string instanceName, string calendarName, CancellationToken cancellationToken = default)
        {
            return dataContext.BulkDeleteAsync<Model.ICalendar>(c => c.InstanceName == instanceName && c.CalendarName == calendarName, cancellationToken: cancellationToken);
        }

        public static async Task AddScheduler(this IDataContext dataContext, string instanceName, string instanceId, CancellationToken cancellationToken = default)
        {
            var scheduler = await dataContext.CreateEntityAsync<Model.IScheduler>(cancellationToken: cancellationToken).PreserveThreadContext();
            scheduler.InstanceName = instanceName;
            scheduler.InstanceId = instanceId;
            scheduler.State = SchedulerState.Started;
            scheduler.LastCheckIn = DateTime.Now;

            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
        }

        public static Task DeleteScheduler(this IDataContext dataContext, string instanceName, string instanceId, CancellationToken cancellationToken = default)
        {
            return dataContext.BulkDeleteAsync<Model.IScheduler>(s => s.InstanceName == instanceName && s.InstanceId == instanceId, cancellationToken: cancellationToken);
        }

        public static async Task UpdateSchedulerState(this IDataContext dataContext, string instanceName, string instanceId, SchedulerState state, CancellationToken cancellationToken = default)
        {
            // TODO change with BulkUpdate
            var scheduler = await dataContext.FindOneAsync<Model.IScheduler>(
                                s => s.InstanceName == instanceName && s.InstanceId == instanceId,
                                cancellationToken: cancellationToken).PreserveThreadContext();
            scheduler.State = state;
            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
        }

        public static async Task<List<string>> GetPausedTriggerGroups(this IDataContext dataContext, string instanceName, CancellationToken cancellationToken = default)
        {
            var groups = await dataContext.Query<IPausedTriggerGroup>()
                             .Where(g => g.InstanceName == instanceName)
                             .ToListAsync(cancellationToken: cancellationToken)
                             .PreserveThreadContext();
            return groups.Select(g => g.Group).Distinct().ToList();
        }

        public static Task<bool> IsTriggerGroupPaused(this IDataContext dataContext, string instanceName, string group, CancellationToken cancellationToken = default)
        {
            return dataContext.Query<IPausedTriggerGroup>()
                .Where(g => g.InstanceName == instanceName && g.Group == group)
                .AnyAsync(cancellationToken: cancellationToken);
        }

        public static async Task AddPausedTriggerGroup(this IDataContext dataContext, string instanceName, string group, CancellationToken cancellationToken = default)
        {
            var entity = await dataContext.CreateEntityAsync<IPausedTriggerGroup>(cancellationToken: cancellationToken).PreserveThreadContext();
            entity.InstanceName = instanceName;
            entity.Group = group;
            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
        }

        public static Task DeletePausedTriggerGroup(this IDataContext dataContext, string instanceName, GroupMatcher<TriggerKey> matcher, CancellationToken cancellationToken = default)
        {
            return dataContext.BulkDeleteAsync<IPausedTriggerGroup>(matcher.ToFilterExpression<IPausedTriggerGroup, TriggerKey>(instanceName), cancellationToken: cancellationToken);
        }

        public static Task DeletePausedTriggerGroup(this IDataContext dataContext, string instanceName, string groupName, CancellationToken cancellationToken = default)
        {
            return dataContext.BulkDeleteAsync<Model.IPausedTriggerGroup>(
                s => s.InstanceName == instanceName && s.Group == groupName,
                cancellationToken: cancellationToken);
        }

        public static async Task<global::Quartz.IJobDetail> GetJobDetail(this IDataContext dataContext, string instanceName, JobKey jobKey, CancellationToken cancellationToken = default)
        {
            var job = await dataContext.GetJob(instanceName, jobKey, cancellationToken).PreserveThreadContext();
            return job?.GetJobDetail();
        }

        public static Task<Model.IJobDetail> GetJob(this IDataContext dataContext, string instanceName, JobKey jobKey, CancellationToken cancellationToken = default)
        {
            return dataContext.FindOneAsync<Model.IJobDetail>(
                j => j.InstanceName == instanceName && j.Group == jobKey.Group && j.Name == jobKey.Name,
                throwIfNotFound: false,
                cancellationToken: cancellationToken);
        }

        public static async Task<List<JobKey>> GetJobsKeys(this IDataContext dataContext, string instanceName, GroupMatcher<JobKey> matcher, CancellationToken cancellationToken = default)
        {
            var jobs = await dataContext.Query<Model.IJobDetail>()
                           .Where(matcher.ToFilterExpression<Model.IJobDetail, JobKey>(instanceName))
                           .ToListAsync(cancellationToken: cancellationToken)
                           .PreserveThreadContext();
            return jobs.Select(j => j.GetJobKey()).ToList();
        }

        public static async Task<IEnumerable<string>> GetJobGroupNames(this IDataContext dataContext, string instanceName, CancellationToken cancellationToken = default)
        {
            var jobs = await dataContext.Query<Model.IJobDetail>()
                           .Where(j => j.InstanceName == instanceName)
                           .ToListAsync(cancellationToken: cancellationToken)
                           .PreserveThreadContext();
            return jobs.Select(j => j.Group).Distinct().ToList();
        }

        public static Task<long> DeleteJob(this IDataContext dataContext, string instanceName, JobKey key, CancellationToken cancellationToken = default)
        {
            return dataContext.BulkDeleteAsync<Model.IJobDetail>(
                s => s.InstanceName == instanceName && s.Group == key.Group && s.Name == key.Name,
                cancellationToken: cancellationToken);
        }

        public static async Task AddJob(this IDataContext dataContext, string instanceName, global::Quartz.IJobDetail jobDetail, CancellationToken cancellationToken = default)
        {
            var job = await dataContext.CreateEntityAsync<Model.IJobDetail>(cancellationToken: cancellationToken).PreserveThreadContext();
            job.InstanceName = instanceName;
            job.Name = jobDetail.Key.Name;
            job.Group = jobDetail.Key.Group;
            job.Description = jobDetail.Description;
            job.JobType = jobDetail.JobType;
            job.JobDataMap = jobDetail.JobDataMap;
            job.Durable = jobDetail.Durable;
            job.PersistJobDataAfterExecution = jobDetail.PersistJobDataAfterExecution;
            job.ConcurrentExecutionDisallowed = jobDetail.ConcurrentExecutionDisallowed;
            job.RequestsRecovery = jobDetail.RequestsRecovery;

            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
        }

        public static async Task<long> UpdateJob(this IDataContext dataContext, string instanceName, global::Quartz.IJobDetail jobDetail, bool upsert, CancellationToken cancellationToken = default)
        {
            var job = await dataContext.FindOneAsync<Model.IJobDetail>(
                j => j.InstanceName == instanceName && j.Group == jobDetail.Key.Group && j.Name == jobDetail.Key.Name,
                throwIfNotFound: !upsert,
                cancellationToken).PreserveThreadContext();

            if (job == null)
            {
                await dataContext.AddJob(instanceName, jobDetail, cancellationToken).PreserveThreadContext();
                return 1;
            }

            job.Description = jobDetail.Description;
            job.JobType = jobDetail.JobType;
            job.JobDataMap = jobDetail.JobDataMap;
            job.Durable = jobDetail.Durable;
            job.PersistJobDataAfterExecution = jobDetail.PersistJobDataAfterExecution;
            job.ConcurrentExecutionDisallowed = jobDetail.ConcurrentExecutionDisallowed;
            job.RequestsRecovery = jobDetail.RequestsRecovery;

            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            return 1;
        }

        public static async Task<bool> TryAcquireLock(this IDataContext dataContext, string instanceName, LockType lockType, string instanceId, CancellationToken cancellationToken = default)
        {
            Log.Trace($"Trying to acquire lock {instanceName}/{lockType} on {instanceId}");
            try
            {
                var entity = await dataContext.CreateEntityAsync<ILock>(cancellationToken: cancellationToken)
                    .PreserveThreadContext();
                entity.InstanceName = instanceName;
                entity.LockType = lockType;
                entity.InstanceId = instanceId;
                entity.AcquiredAt = DateTime.Now;

                await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();

                Log.Trace($"Acquired lock {instanceName}/{lockType} on {instanceId}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Trace(ex, $"Failed to acquire lock {instanceName}/{lockType} on {instanceId}");
                return false;
            }
        }

        public static async Task<bool> ReleaseLock(this IDataContext dataContext, string instanceName, LockType lockType, string instanceId, CancellationToken cancellationToken = default)
        {
            Log.Trace($"Releasing lock {instanceName}/{lockType} on {instanceId}");
            var entity = await dataContext.FindOneAsync<ILock>(
                             l => l.InstanceName == instanceName && l.LockType == lockType && l.InstanceId == instanceId,
                             throwIfNotFound: false,
                             cancellationToken: cancellationToken).PreserveThreadContext();

            if (entity == null)
            {
                Log.Warn($"Failed to release lock {instanceName}/{lockType} on {instanceId}. You do not own the lock.");
                return false;
            }

            dataContext.DeleteEntity(entity);

            await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext();

            Log.Trace($"Released lock {instanceName}/{lockType} on {instanceId}");
            return true;
        }
    }
}