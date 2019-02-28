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

        public async Task<JobDetail> GetJob(JobKey jobKey)
        {
            return await this.Collection.Find(detail => detail.Id == new JobDetailId(jobKey, this.InstanceName)).FirstOrDefaultAsync();
        }

        public async Task<List<JobKey>> GetJobsKeys(GroupMatcher<JobKey> matcher)
        {
            return
                await this.Collection.Find(this.FilterBuilder.And(
                    this.FilterBuilder.Eq(detail => detail.Id.InstanceName, this.InstanceName),
                    this.FilterBuilder.Regex(detail => detail.Id.Group, matcher.ToBsonRegularExpression())))
                    .Project(detail => detail.Id.GetJobKey())
                    .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetJobGroupNames()
        {
            return await this.Collection
                .Distinct(detail => detail.Id.Group, detail => detail.Id.InstanceName == this.InstanceName)
                .ToListAsync();
        } 

        public async Task AddJob(JobDetail jobDetail)
        {
            await this.Collection.InsertOneAsync(jobDetail);
        }

        public async Task<long> UpdateJob(JobDetail jobDetail, bool upsert)
        {
            var result = await this.Collection.ReplaceOneAsync(detail => detail.Id == jobDetail.Id,
                jobDetail,
                new UpdateOptions
                {
                    IsUpsert = upsert
                });
            return result.ModifiedCount;
        }

        public async Task UpdateJobData(JobKey jobKey, JobDataMap jobDataMap)
        {
            await this.Collection.UpdateOneAsync(detail => detail.Id == new JobDetailId(jobKey, this.InstanceName),
                this.UpdateBuilder.Set(detail => detail.JobDataMap, jobDataMap));
        }

        public async Task<long> DeleteJob(JobKey key)
        {
            var result = await this.Collection.DeleteOneAsync(this.FilterBuilder.Where(job => job.Id == new JobDetailId(key, this.InstanceName)));
            return result.DeletedCount;
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