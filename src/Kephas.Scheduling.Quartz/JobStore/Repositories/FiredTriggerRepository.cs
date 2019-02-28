// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiredTriggerRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the fired trigger repository class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Quartz;

    using Kephas.Scheduling.Quartz.JobStore.Models;
    using Kephas.Scheduling.Quartz.JobStore.Models.Identifiers;

    //TODO [CollectionName("firedTriggers")]
    internal class FiredTriggerRepository : BaseRepository<FiredTrigger>
    {
        public FiredTriggerRepository(IMongoDatabase database, string instanceName, string collectionPrefix = null)
            : base(database, instanceName)
        {
        }

        public async Task<List<FiredTrigger>> GetFiredTriggers(JobKey jobKey)
        {
            return
                await this.Collection.Find(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.JobKey == jobKey).ToListAsync();
        }

        public async Task<List<FiredTrigger>> GetFiredTriggers(string instanceId)
        {
            return
                await this.Collection.Find(trigger => trigger.Id.InstanceName == this.InstanceName && trigger.InstanceId == instanceId)
                    .ToListAsync();
        }

        public async Task<List<FiredTrigger>> GetRecoverableFiredTriggers(string instanceId)
        {
            return
                await this.Collection.Find(
                    trigger =>
                        trigger.Id.InstanceName == this.InstanceName && trigger.InstanceId == instanceId &&
                        trigger.RequestsRecovery).ToListAsync();
        }

        public async Task AddFiredTrigger(FiredTrigger firedTrigger)
        {
            await this.Collection.InsertOneAsync(firedTrigger);
        }

        public async Task DeleteFiredTrigger(string firedInstanceId)
        {
            await this.Collection.DeleteOneAsync(trigger => trigger.Id == new FiredTriggerId(firedInstanceId, this.InstanceName));
        }

        public async Task<long> DeleteFiredTriggersByInstanceId(string instanceId)
        {
            var result =
                await this.Collection.DeleteManyAsync(
                    trigger => trigger.Id.InstanceName == this.InstanceName && trigger.InstanceId == instanceId);
            return result.DeletedCount;
        }

        public async Task UpdateFiredTrigger(FiredTrigger firedTrigger)
        {
            await this.Collection.ReplaceOneAsync(trigger => trigger.Id == firedTrigger.Id, firedTrigger);
        }
    }
}