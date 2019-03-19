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
                return await dataContext.FindOneAsync<Model.ITrigger>(
                           t => t.InstanceName == this.InstanceName && t.Name == key.Name && t.Group == key.Group,
                           cancellationToken: cancellationToken).PreserveThreadContext();
            }
        }

        public async Task<List<Model.ITrigger>> GetTriggers(string calendarName)
        {
            return await this.Collection.Find(this.FilterBuilder.Where(trigger => trigger.CalendarName == calendarName)).ToListAsync();
        }

        public async Task<List<Model.ITrigger>> GetTriggers(JobKey jobKey)
        {
            return
                await this.Collection.Find(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey).ToListAsync();
        }

        public async Task<List<TriggerKey>> GetTriggerKeys(GroupMatcher<TriggerKey> matcher)
        {
            return await this.Collection.Find(this.FilterBuilder.And(
                this.FilterBuilder.Eq(trigger => trigger.Id.InstanceName, this.InstanceName),
                this.FilterBuilder.Regex(trigger => trigger.Id.Group, matcher.ToBsonRegularExpression())))
                .Project(trigger => trigger.Id.GetTriggerKey())
                .ToListAsync();
        }

        public async Task<List<TriggerKey>> GetTriggerKeys(Model.TriggerState state)
        {
            return await this.Collection.Find(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.State == state)
                .Project(trigger => trigger.Id.GetTriggerKey())
                .ToListAsync();
        }

        public async Task<List<string>> GetTriggerGroupNames()
        {
            return await this.Collection.Distinct(trigger => trigger.Id.Group,
                trigger => trigger.Id.InstanceName == this.InstanceName)
                .ToListAsync();
        }

        public async Task<List<string>> GetTriggerGroupNames(GroupMatcher<TriggerKey> matcher)
        {
            var regex = matcher.ToBsonRegularExpression().ToRegex();
            return await this.Collection.Distinct(trigger => trigger.Id.Group,
                    trigger => trigger.Id.InstanceName == this.InstanceName && regex.IsMatch(trigger.Id.Group))
                .ToListAsync();
        }

        public async Task<List<TriggerKey>> GetTriggersToAcquire(DateTimeOffset noLaterThan, DateTimeOffset noEarlierThan,
            int maxCount)
        {
            if (maxCount < 1)
            {
                maxCount = 1;
            }

            var noLaterThanDateTime = noLaterThan.UtcDateTime;
            var noEarlierThanDateTime = noEarlierThan.UtcDateTime;

            return await this.Collection.Find(trigger => trigger.Id.InstanceName == this.InstanceName &&
                                              trigger.State == Model.TriggerState.Waiting &&
                                              trigger.NextFireTime <= noLaterThanDateTime &&
                                              (trigger.MisfireInstruction == -1 ||
                                               (trigger.MisfireInstruction != -1 &&
                                                trigger.NextFireTime >= noEarlierThanDateTime)))
                .Sort(this.SortBuilder.Combine(
                    this.SortBuilder.Ascending(trigger => trigger.NextFireTime),
                    this.SortBuilder.Descending(trigger => trigger.Priority)
                    ))
                .Limit(maxCount)
                .Project(trigger => trigger.Id.GetTriggerKey())
                .ToListAsync();
        }

        public async Task<long> GetCount()
        {
            return await this.Collection.Find(trigger => trigger.Id.InstanceName == this.InstanceName).CountAsync();
        }

        public async Task<long> GetCount(JobKey jobKey)
        {
            return
                await this.Collection.Find(
                    this.FilterBuilder.Where(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey))
                    .CountAsync();
        }

        public async Task<long> GetMisfireCount(DateTime nextFireTime)
        {
            return
                await this.Collection.Find(
                    trigger =>
                        trigger.Id.InstanceName == this.InstanceName &&
                        trigger.MisfireInstruction != MisfireInstruction.IgnoreMisfirePolicy &&
                        trigger.NextFireTime < nextFireTime && trigger.State == Model.TriggerState.Waiting)
                    .CountAsync();
        }

        public async Task AddTrigger(Model.ITrigger trigger)
        {
            await this.Collection.InsertOneAsync(trigger);
        }

        public async Task UpdateTrigger(Model.ITrigger trigger)
        {
            await this.Collection.ReplaceOneAsync(t => t.Id == trigger.Id, trigger);
        }

        public async Task<long> UpdateTriggerState(TriggerKey triggerKey, Model.TriggerState state)
        {
            var result = await this.Collection.UpdateOneAsync(trigger => trigger.Id == new TriggerId(triggerKey, this.InstanceName),
                this.UpdateBuilder.Set(trigger => trigger.State, state));
            return result.ModifiedCount;
        }

        public async Task<long> UpdateTriggerState(TriggerKey triggerKey, Model.TriggerState newState, Model.TriggerState oldState)
        {
            var result = await this.Collection.UpdateOneAsync(
                trigger => trigger.Id == new TriggerId(triggerKey, this.InstanceName) && trigger.State == oldState,
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
        }

        public async Task<long> UpdateTriggersStates(GroupMatcher<TriggerKey> matcher, Model.TriggerState newState,
            params Model.TriggerState[] oldStates)
        {
            var result = await this.Collection.UpdateManyAsync(this.FilterBuilder.And(
                this.FilterBuilder.Eq(trigger => trigger.Id.InstanceName, this.InstanceName),
                this.FilterBuilder.Regex(trigger => trigger.Id.Group, matcher.ToBsonRegularExpression()),
                this.FilterBuilder.In(trigger => trigger.State, oldStates)),
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
        }

        public async Task<long> UpdateTriggersStates(JobKey jobKey, Model.TriggerState newState,
            params Model.TriggerState[] oldStates)
        {
            var result = await this.Collection.UpdateManyAsync(
                trigger =>
                    trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey &&
                    oldStates.Contains(trigger.State),
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
        }

        public async Task<long> UpdateTriggersStates(JobKey jobKey, Model.TriggerState newState)
        {
            var result = await this.Collection.UpdateManyAsync(
                trigger =>
                    trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey,
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
        }

        public async Task<long> UpdateTriggersStates(Model.TriggerState newState, params Model.TriggerState[] oldStates)
        {
            var result = await this.Collection.UpdateManyAsync(
                trigger =>
                    trigger.Id.InstanceName == this.InstanceName && oldStates.Contains(trigger.State),
                this.UpdateBuilder.Set(trigger => trigger.State, newState));
            return result.ModifiedCount;
        }

        public async Task<long> DeleteTrigger(TriggerKey key)
        {
            var result =
                await this.Collection.DeleteOneAsync(this.FilterBuilder.Where(trigger => trigger.Id == new TriggerId(key, this.InstanceName)));
            return result.DeletedCount;
        }

        public async Task<long> DeleteTriggers(JobKey jobKey)
        {
            var result = await this.Collection.DeleteManyAsync(
                this.FilterBuilder.Where(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey));
            return result.DeletedCount;
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
        }
    }
}