// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFiredTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFiredTrigger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using System;
    using System.Globalization;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;
    using global::Quartz.Spi;

    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Interface for fired trigger.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(FiredInstanceId) })]
    public interface IFiredTrigger : IEntityBase
    {
        /// <summary>
        /// Gets or sets the identifier of the fired instance.
        /// </summary>
        /// <value>
        /// The identifier of the fired instance.
        /// </value>
        string FiredInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the trigger.
        /// </summary>
        /// <value>
        /// The name of the trigger.
        /// </value>
        string TriggerName { get; set; }

        /// <summary>
        /// Gets or sets the group the trigger belongs to.
        /// </summary>
        /// <value>
        /// The trigger group.
        /// </value>
        string TriggerGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        /// <value>
        /// The name of the job.
        /// </value>
        string JobName { get; set; }

        /// <summary>
        /// Gets or sets the group the job belongs to.
        /// </summary>
        /// <value>
        /// The job group.
        /// </value>
        string JobGroup { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the instance.
        /// </summary>
        /// <value>
        /// The identifier of the instance.
        /// </value>
        string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the fired time.
        /// </summary>
        /// <value>
        /// The fired time.
        /// </value>
        DateTime Fired { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the scheduled time.
        /// </summary>
        /// <value>
        /// The scheduled time.
        /// </value>
        DateTime? Scheduled { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        int Priority { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        Model.TriggerState State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the concurrent execution is disallowed.
        /// </summary>
        /// <value>
        /// True if concurrent execution is disallowed, false if not.
        /// </value>
        bool ConcurrentExecutionDisallowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the recovery is requested.
        /// </summary>
        /// <value>
        /// True if recovery is requested, false if not.
        /// </value>
        bool RequestsRecovery { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IFiredTrigger"/>.
    /// </summary>
    public static class FiredTriggerExtensions
    {
        /// <summary>
        /// Gets the job key.
        /// </summary>
        /// <param name="firedTrigger">The firedTrigger to act on.</param>
        /// <returns>
        /// The job key.
        /// </returns>
        public static JobKey GetJobKey(this IFiredTrigger firedTrigger)
        {
            return new JobKey(firedTrigger.JobName, firedTrigger.JobGroup);
        }

        /// <summary>
        /// Gets the trigger key.
        /// </summary>
        /// <param name="firedTrigger">The firedTrigger to act on.</param>
        /// <returns>
        /// The trigger key.
        /// </returns>
        public static TriggerKey GetTriggerKey(this IFiredTrigger firedTrigger)
        {
            return new TriggerKey(firedTrigger.TriggerName, firedTrigger.TriggerGroup);
        }

        /// <summary>
        /// Gets the recovery trigger.
        /// </summary>
        /// <param name="firedTrigger">The firedTrigger to act on.</param>
        /// <param name="jobDataMap">The job data map.</param>
        /// <returns>
        /// The recovery trigger.
        /// </returns>
        public static IOperableTrigger GetRecoveryTrigger(this IFiredTrigger firedTrigger, JobDataMap jobDataMap)
        {
            var firedTime = new DateTimeOffset(firedTrigger.Fired);
            var scheduledTime = firedTrigger.Scheduled.HasValue ? new DateTimeOffset(firedTrigger.Scheduled.Value) : DateTimeOffset.MinValue;
            var recoveryTrigger = new SimpleTriggerImpl($"recover_{firedTrigger.InstanceId}_{Guid.NewGuid()}",
                                      SchedulerConstants.DefaultRecoveryGroup, scheduledTime)
            {
                JobName = firedTrigger.JobName,
                JobGroup = firedTrigger.JobGroup,
                Priority = firedTrigger.Priority,
                MisfireInstruction = MisfireInstruction.IgnoreMisfirePolicy,
                JobDataMap = jobDataMap
            };
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerName, firedTrigger.TriggerName);
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerGroup, firedTrigger.TriggerGroup);
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerFiretime,
                Convert.ToString(firedTime, CultureInfo.InvariantCulture));
            recoveryTrigger.JobDataMap.Put(SchedulerConstants.FailedJobOriginalTriggerScheduledFiretime,
                Convert.ToString(scheduledTime, CultureInfo.InvariantCulture));
            return recoveryTrigger;
        }
    }
}