// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobDetailRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job detail repository class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Quartz;
    using global::Quartz.Impl.Matchers;

    using Kephas.Scheduling.Quartz.JobStore.Models;
    using Kephas.Scheduling.Quartz.JobStore.Models.Identifiers;

    //TODO [CollectionName("jobs")]
    internal class JobDetailRepository : BaseRepository<JobDetail>
    {
        public JobDetailRepository(IMongoDatabase database, string instanceName)
            : base(database, instanceName)
        {
        }

        public async Task UpdateJobData(JobKey jobKey, JobDataMap jobDataMap)
        {
            await this.Collection.UpdateOneAsync(detail => detail.Id == new JobDetailId(jobKey, this.InstanceName),
                this.UpdateBuilder.Set(detail => detail.JobDataMap, jobDataMap));
        }

        public async Task<bool> JobExists(JobKey jobKey)
        {
            return await this.Collection.Find(detail => detail.Id == new JobDetailId(jobKey, this.InstanceName)).AnyAsync();
        }

        public async Task<long> GetCount()
        {
            return await this.Collection.Find(detail => detail.Id.InstanceName == this.InstanceName).CountAsync();
        }
    }
}