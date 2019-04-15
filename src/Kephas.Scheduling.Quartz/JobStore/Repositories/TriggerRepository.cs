// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the trigger repository class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Quartz;
    using global::Quartz.Impl.Matchers;

    using Kephas.Data;
    using Kephas.Data.Linq;
    using Kephas.Logging;
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Scheduling.Quartz.Linq;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A trigger repository.
    /// </summary>
    internal class TriggerRepository
    {
        /// <summary>
        /// The job store.
        /// </summary>
        private readonly ISchedulingJobStore jobStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerRepository"/> class.
        /// </summary>
        /// <param name="jobStore">The job store.</param>
        public TriggerRepository(ISchedulingJobStore jobStore)
        {
            this.jobStore = jobStore;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<TriggerRepository> Log { get; set; }

        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        /// <value>
        /// The name of the instance.
        /// </value>
        public string InstanceName => this.jobStore.InstanceName;

        /// <summary>
        /// Queries if a given trigger exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields true if it succeeds, false if it fails.
        /// </returns>
        public async Task<bool> TriggerExists(TriggerKey key, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.jobStore.DataContextFactory(null))
            {
                return await dataContext.Query<Model.ITrigger>().Where(
                               trigger => trigger.InstanceName == this.InstanceName
                                          && trigger.Name == key.Name
                                          && trigger.Group == key.Group)
                           .AnyAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Queries if a given triggers exists.
        /// </summary>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields true if it succeeds, false if it fails.
        /// </returns>
        public async Task<bool> TriggersExists(string calendarName, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.jobStore.DataContextFactory(null))
            {
                return await dataContext.Query<Model.ITrigger>().Where(
                               trigger => trigger.InstanceName == this.InstanceName
                                          && trigger.CalendarName == calendarName)
                           .AnyAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the trigger based on the key.
        /// </summary>
        /// <param name="key">The trigger key.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the trigger.
        /// </returns>
        public async Task<Model.ITrigger> GetTrigger(TriggerKey key, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.jobStore.DataContextFactory(null))
            {
                return await this.GetTrigger(dataContext, key, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the trigger based on the key.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="key">The trigger key.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the trigger.
        /// </returns>
        public Task<Model.ITrigger> GetTrigger(IDataContext dataContext, TriggerKey key, CancellationToken cancellationToken = default)
        {
            return dataContext.FindOneAsync<Model.ITrigger>(
                       t => t.InstanceName == this.InstanceName && t.Name == key.Name && t.Group == key.Group,
                       cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets the triggers based on calendar name.
        /// </summary>
        /// <param name="calendarName">Name of the calendar.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the triggers.
        /// </returns>
        public async Task<IList<Model.ITrigger>> GetTriggers(string calendarName, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.jobStore.DataContextFactory(null))
            {
                return await dataContext.Query<Model.ITrigger>().Where(
                           t => t.InstanceName == this.InstanceName && t.CalendarName == calendarName)
                           .ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Gets the triggers based on job key.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="jobKey">The job key.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the triggers.
        /// </returns>
        public Task<IList<Model.ITrigger>> GetTriggers(IDataContext dataContext, JobKey jobKey, CancellationToken cancellationToken = default)
        {
            return dataContext.Query<Model.ITrigger>().Where(
                           t => t.InstanceName == this.InstanceName && t.JobName == jobKey.Name && t.JobGroup == jobKey.Group)
                       .ToListAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets the trigger keys based on the provided matcher.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the trigger keys.
        /// </returns>
        public async Task<IList<TriggerKey>> GetTriggerKeys(GroupMatcher<TriggerKey> matcher, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.jobStore.DataContextFactory(null))
            {
                var triggers = await dataContext.Query<Model.ITrigger>()
                    .Where(t => t.InstanceName == this.InstanceName)
                    .Where(matcher.ToFilterExpression<Model.ITrigger, TriggerKey>(this.InstanceName))
                    .ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();
                return triggers.Select(t => t.GetTriggerKey()).ToList();
            }
        }

        /// <summary>
        /// Gets the trigger keys based on the provided state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the trigger keys.
        /// </returns>
        public async Task<List<TriggerKey>> GetTriggerKeys(Model.TriggerState state, CancellationToken cancellationToken = default)
        {
            using (var dataContext = this.jobStore.DataContextFactory(null))
            {
                var triggers = await dataContext.Query<Model.ITrigger>()
                                   .Where(t => t.InstanceName == this.InstanceName && t.State == state)
                                   .ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();
                return triggers.Select(t => t.GetTriggerKey()).ToList();
            }
        }

        /// <summary>
        /// Gets trigger group names.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the trigger group names.
        /// </returns>
        public async Task<IList<string>> GetTriggerGroupNames(
            IDataContext dataContext,
            CancellationToken cancellationToken = default)
        {
            var groups = await dataContext.Query<Model.ITrigger>()
                             .Where(t => t.InstanceName == this.InstanceName)
                             .Select(t => t.Group)
                             .Distinct()
                             .ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            return groups;
        }

        /// <summary>
        /// Gets trigger group names.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="matcher">The matcher.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the trigger group names.
        /// </returns>
        public async Task<IList<string>> GetTriggerGroupNames(IDataContext dataContext, GroupMatcher<TriggerKey> matcher, CancellationToken cancellationToken = default)
        {
            var groups = await dataContext.Query<Model.ITrigger>()
                               .Where(t => t.InstanceName == this.InstanceName)
                               .Where(matcher.ToFilterExpression<Model.ITrigger, TriggerKey>(this.InstanceName))
                               .Select(t => t.Group)
                               .Distinct()
                               .ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            return groups;
        }

        /// <summary>
        /// Gets triggers to acquire.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="noLaterThan">The no later than.</param>
        /// <param name="noEarlierThan">The no earlier than.</param>
        /// <param name="maxCount">Number of maximums.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the triggers to acquire.
        /// </returns>
        public async Task<List<TriggerKey>> GetTriggersToAcquire(IDataContext dataContext, DateTimeOffset noLaterThan, DateTimeOffset noEarlierThan, int maxCount, CancellationToken cancellationToken = default)
        {
            if (maxCount < 1)
            {
                maxCount = 1;
            }

            var noLaterThanDateTime = noLaterThan.UtcDateTime;
            var noEarlierThanDateTime = noEarlierThan.UtcDateTime;

            var triggers = await dataContext.Query<Model.ITrigger>().Where(
                                   trigger => trigger.InstanceName == this.InstanceName
                                              && trigger.State == Model.TriggerState.Waiting
                                              && trigger.NextFireTime <= noLaterThanDateTime
                                              && (trigger.MisfireInstruction == -1
                                                  || (trigger.MisfireInstruction != -1
                                                      && trigger.NextFireTime >= noEarlierThanDateTime)))
                               .OrderBy(trigger => trigger.NextFireTime)
                               .ThenByDescending(trigger => trigger.Priority)
                               .Take(maxCount)
                               .ToListAsync(cancellationToken: cancellationToken).PreserveThreadContext();

            return triggers.Select(trigger => trigger.GetTriggerKey()).ToList();
        }

        /// <summary>
        /// Gets a count.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the count.
        /// </returns>
        public Task<long> GetCount(IDataContext dataContext, CancellationToken cancellationToken = default)
        {
            return dataContext.Query<Model.ITrigger>()
                .Where(trigger => trigger.InstanceName == this.InstanceName)
                .LongCountAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets a count.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="jobKey">The job key.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the count.
        /// </returns>
        public Task<long> GetCount(IDataContext dataContext, JobKey jobKey, CancellationToken cancellationToken = default)
        {
            return dataContext.Query<Model.ITrigger>()
                .Where(trigger => trigger.InstanceName == this.InstanceName && trigger.JobGroup == jobKey.Group && trigger.JobName == jobKey.Name)
                .LongCountAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets misfire count.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="nextFireTime">.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the misfire count.
        /// </returns>
        public Task<long> GetMisfireCount(IDataContext dataContext, DateTime nextFireTime, CancellationToken cancellationToken = default)
        {
            return dataContext.Query<Model.ITrigger>()
                .Where(trigger => trigger.InstanceName == this.InstanceName && trigger.MisfireInstruction != MisfireInstruction.IgnoreMisfirePolicy
                                                                            && trigger.NextFireTime < nextFireTime
                                                                            && trigger.State == Model.TriggerState.Waiting)
                .LongCountAsync(cancellationToken: cancellationToken);
        }

        public async Task AddTrigger(Model.ITrigger trigger)
        {
            return; // TODO await this.Collection.InsertOneAsync(trigger);
        }

        public async Task UpdateTrigger(Model.ITrigger trigger)
        {
            return; // TODO await this.Collection.ReplaceOneAsync(t => t.Id == trigger.Id, trigger);
        }

        public async Task<long> UpdateTriggerState(IDataContext dataContext, TriggerKey triggerKey, Model.TriggerState state, CancellationToken cancellationToken = default)
        {
            return 0;
            /* TODO
            var result = await this.Collection.UpdateOneAsync(trigger => trigger.Id == new TriggerId(triggerKey, this.InstanceName),
                this.UpdateBuilder.Set(trigger => trigger.State, state));
            return result.ModifiedCount;
            */
        }

        public async Task<long> UpdateTriggerState(TriggerKey triggerKey, Model.TriggerState newState, Model.TriggerState oldState)
        {
            return 0;
            /* TODO
            var result = await this.Collection.UpdateOneAsync(
                trigger => trigger.Id == new TriggerId(triggerKey, this.InstanceName) && trigger.State == oldState,
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
            */
        }

        public async Task<long> UpdateTriggersStates(GroupMatcher<TriggerKey> matcher, Model.TriggerState newState,
            params Model.TriggerState[] oldStates)
        {
            return 0;
            /* TODO
            var result = await this.Collection.UpdateManyAsync(this.FilterBuilder.And(
                this.FilterBuilder.Eq(trigger => trigger.Id.InstanceName, this.InstanceName),
                this.FilterBuilder.Regex(trigger => trigger.Id.Group, matcher.ToBsonRegularExpression()),
                this.FilterBuilder.In(trigger => trigger.State, oldStates)),
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
            */
        }

        public async Task<long> UpdateTriggersStates(JobKey jobKey, Model.TriggerState newState,
            params Model.TriggerState[] oldStates)
        {
            return 0;
            /* TODO
            var result = await this.Collection.UpdateManyAsync(
                trigger =>
                    trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey &&
                    oldStates.Contains(trigger.State),
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
            */
        }

        public async Task<long> UpdateTriggersStates(JobKey jobKey, Model.TriggerState newState)
        {
            return 0;
            /* TODO
            var result = await this.Collection.UpdateManyAsync(
                trigger =>
                    trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey,
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
            */
        }

        public async Task<long> UpdateTriggersStates(Model.TriggerState newState, params Model.TriggerState[] oldStates)
        {
            return 0;
            /* TODO
            var result = await this.Collection.UpdateManyAsync(
                trigger =>
                    trigger.Id.InstanceName == this.InstanceName && oldStates.Contains(trigger.State),
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
            */
        }

        public async Task<long> DeleteTrigger(TriggerKey key)
        {
            return 0;
            /* TODO
            var result =
                await this.Collection.DeleteOneAsync(this.FilterBuilder.Where(trigger => trigger.Id == new TriggerId(key, this.InstanceName)));
            return result.DeletedCount;
            */
        }

        public async Task<long> DeleteTriggers(JobKey jobKey)
        {
            return 0;
            /* TODO
            var result = await this.Collection.DeleteManyAsync(
                this.FilterBuilder.Where(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey));
            return result.DeletedCount;
            */
        }

        /// <summary>
        /// Get the names of all of the triggers in the given state that have
        /// misfired - according to the given timestamp.  No more than count will
        /// be returned.
        /// </summary>
        /// <param name="nextFireTime"></param>
        /// <param name="maxResults"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public bool HasMisfiredTriggers(DateTime nextFireTime, int maxResults, out List<TriggerKey> results)
        {
            results = null; return false;
            /* TODO
            var cursor = this.Collection.Find(
                trigger => trigger.Id.InstanceName == this.InstanceName &&
                           trigger.MisfireInstruction != MisfireInstruction.IgnoreMisfirePolicy &&
                           trigger.NextFireTime < nextFireTime &&
                           trigger.State == Model.TriggerState.Waiting)
                .Project(trigger => trigger.Id.GetTriggerKey())
                .Sort(this.SortBuilder.Combine(
                    this.SortBuilder.Ascending(trigger => trigger.NextFireTime),
                    this.SortBuilder.Descending(trigger => trigger.Priority)
                    )).ToCursor();

            results = new List<TriggerKey>();

            var hasReachedLimit = false;
            while (cursor.MoveNext() && !hasReachedLimit)
            {
                foreach (var triggerKey in cursor.Current)
                {
                    if (results.Count == maxResults)
                    {
                        hasReachedLimit = true;
                    }
                    else
                    {
                        results.Add(triggerKey);
                    }
                }
            }
            return hasReachedLimit;
            */
        }
    }
}