// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITrigger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using System;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    using Kephas.Data.Model.AttributedModel;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Values that represent trigger states.
    /// </summary>
    /// <seealso/>
    public enum TriggerState
    {
        None = 0,
        Waiting,
        Acquired,
        Executing,
        Complete,
        Blocked,
        Error,
        Paused,
        PausedBlocked,
        Deleted
    }

    /// <summary>
    /// Interface for trigger.
    /// </summary>
    [Abstract]
    [NaturalKey(new[] { nameof(InstanceName), nameof(Group), nameof(Name) })]
    public interface ITrigger : IGroupedEntityBase, INamedEntityBase
    {
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; set; }

        DateTime? NextFireTime { get; set; }

        DateTime? PreviousFireTime { get; set; }

        TriggerState State { get; set; }

        DateTime StartTime { get; set; }

        DateTime? EndTime { get; set; }

        string CalendarName { get; set; }

        int MisfireInstruction { get; set; }

        int Priority { get; set; }

        string Type { get; set; }

        JobDataMap JobDataMap { get; set; }

        global::Quartz.ITrigger GetTrigger();
    }

    /// <summary>
    /// Extension methods for <see cref="ITrigger"/>.
    /// </summary>
    public static class TriggerExtensions
    {
        /// <summary>
        /// Gets the trigger key.
        /// </summary>
        /// <param name="trigger">The trigger to act on.</param>
        /// <returns>
        /// The trigger key.
        /// </returns>
        public static TriggerKey GetTriggerKey(this ITrigger trigger)
        {
            return new TriggerKey(trigger.Name, trigger.Group);
        }

        /// <summary>
        /// Gets the job key.
        /// </summary>
        /// <param name="trigger">The trigger to act on.</param>
        /// <returns>
        /// The job key.
        /// </returns>
        public static JobKey GetJobKey(this ITrigger trigger)
        {
            return new JobKey(trigger.JobName, trigger.JobGroup);
        }

        /// <summary>
        /// Fills the native Quartz trigger trigger.
        /// </summary>
        /// <param name="trigger">The trigger to act on.</param>
        /// <param name="quartzTrigger">The quartz trigger.</param>
        internal static void FillTrigger(this ITrigger trigger, AbstractTrigger quartzTrigger)
        {
            quartzTrigger.Key = trigger.GetTriggerKey();
            quartzTrigger.JobKey = trigger.GetJobKey();
            quartzTrigger.CalendarName = trigger.CalendarName;
            quartzTrigger.Description = trigger.Description;
            quartzTrigger.JobDataMap = trigger.JobDataMap;
            quartzTrigger.MisfireInstruction = trigger.MisfireInstruction;
            quartzTrigger.EndTimeUtc = trigger.EndTime;
            quartzTrigger.StartTimeUtc = trigger.StartTime;
            quartzTrigger.Priority = trigger.Priority;
            quartzTrigger.SetNextFireTimeUtc(trigger.NextFireTime);
            quartzTrigger.SetPreviousFireTimeUtc(trigger.PreviousFireTime);
        }
    }
}