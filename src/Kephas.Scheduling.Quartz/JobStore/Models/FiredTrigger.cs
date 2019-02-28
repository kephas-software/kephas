// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiredTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the fired trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;
    using System.Globalization;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;
    using global::Quartz.Spi;

    using Kephas.Data;
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Scheduling.Quartz.JobStore.Models.Identifiers;

    public class FiredTrigger : QuartzEntityBase, IFiredTrigger
    {
        public FiredTrigger()
        {
        }

        public FiredTrigger(string firedInstanceId, Model.ITrigger trigger, JobDetail jobDetail)
        {
            this.Id = new FiredTriggerId(firedInstanceId, trigger.InstanceName);
            this.TriggerKey = trigger.GetTriggerKey();
            this.Fired = DateTime.UtcNow;
            this.Scheduled = trigger.NextFireTime;
            this.Priority = trigger.Priority;
            this.State = trigger.State;

            if (jobDetail != null)
            {
                this.JobKey = jobDetail.GetJobKey();
                this.ConcurrentExecutionDisallowed = jobDetail.ConcurrentExecutionDisallowed;
                this.RequestsRecovery = jobDetail.RequestsRecovery;
            }
        }

        object IIdentifiable.Id => this.Id;

        //TODO [BsonId]
        public FiredTriggerId Id { get; set; }

        public TriggerKey TriggerKey { get; set; }

        public JobKey JobKey { get; set; }

        public string InstanceId { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Fired { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Scheduled { get; set; }

        public int Priority { get; set; }

        //TODO [BsonRepresentation(BsonType.String)]
        public Model.TriggerState State { get; set; }

        public bool ConcurrentExecutionDisallowed { get; set; }

        public bool RequestsRecovery { get; set; }

        public IOperableTrigger GetRecoveryTrigger(JobDataMap jobDataMap)
        {
            var firedTime = new DateTimeOffset(this.Fired);
            var scheduledTime = this.Scheduled.HasValue ? new DateTimeOffset(this.Scheduled.Value) : DateTimeOffset.MinValue;
            var recoveryTrigger = new SimpleTriggerImpl($"recover_{this.InstanceId}_{Guid.NewGuid()}",
                SchedulerConstants.DefaultRecoveryGroup, scheduledTime)
            {
                JobName = this.JobKey.Name,
                JobGroup = this.JobKey.Group,
                Priority = this.Priority,
                MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy,
                JobDataMap = jobDataMap
            };
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerName, this.TriggerKey.Name);
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerGroup, this.TriggerKey.Group);
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerFiretime,
                Convert.ToString(firedTime, CultureInfo.InvariantCulture));
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerScheduledFiretime,
                Convert.ToString(scheduledTime, CultureInfo.InvariantCulture));
            return recoveryTrigger;
        }
    }
}