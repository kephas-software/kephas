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
        /// <summary>
        /// An enum constant representing the none option.
        /// </summary>
        None = 0,

        /// <summary>
        /// An enum constant representing the waiting option.
        /// </summary>
        Waiting,

        /// <summary>
        /// An enum constant representing the acquired option.
        /// </summary>
        Acquired,

        /// <summary>
        /// An enum constant representing the executing option.
        /// </summary>
        Executing,

        /// <summary>
        /// An enum constant representing the complete option.
        /// </summary>
        Complete,

        /// <summary>
        /// An enum constant representing the blocked option.
        /// </summary>
        Blocked,

        /// <summary>
        /// An enum constant representing the error option.
        /// </summary>
        Error,

        /// <summary>
        /// An enum constant representing the paused option.
        /// </summary>
        Paused,

        /// <summary>
        /// An enum constant representing the paused blocked option.
        /// </summary>
        PausedBlocked,

        /// <summary>
        /// An enum constant representing the deleted option.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the next fire time.
        /// </summary>
        /// <value>
        /// The next fire time.
        /// </value>
        DateTime? NextFireTime { get; set; }

        /// <summary>
        /// Gets or sets the previous fire time.
        /// </summary>
        /// <value>
        /// The previous fire time.
        /// </value>
        DateTime? PreviousFireTime { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        TriggerState State { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the calendar.
        /// </summary>
        /// <value>
        /// The name of the calendar.
        /// </value>
        string CalendarName { get; set; }

        /// <summary>
        /// Gets or sets the misfire instruction.
        /// </summary>
        /// <value>
        /// The misfire instruction.
        /// </value>
        int MisfireInstruction { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        int Priority { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        string Type { get; set; }

        /// <summary>
        /// Gets or sets the job data map.
        /// </summary>
        /// <value>
        /// The job data map.
        /// </value>
        JobDataMap JobDataMap { get; set; }

        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="state">The state.</param>
        /// <param name="instanceName">Name of the instance.</param>
        void Initialize(global::Quartz.ITrigger trigger, Model.TriggerState state, string instanceName);

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        /// <returns>
        /// The trigger.
        /// </returns>
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
        public static void FillTrigger(this ITrigger trigger, AbstractTrigger quartzTrigger)
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