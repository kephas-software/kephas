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
        JobKey JobKey { get; set; }

        string Description { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        DateTime? NextFireTime { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        DateTime? PreviousFireTime { get; set; }

        //TODO [BsonRepresentation(BsonType.String)]
        Model.TriggerState State { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        DateTime StartTime { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        DateTime? EndTime { get; set; }

        string CalendarName { get; set; }

        int MisfireInstruction { get; set; }

        int Priority { get; set; }

        string Type { get; set; }

        JobDataMap JobDataMap { get; set; }

        global::Quartz.ITrigger GetTrigger();
    }

    public static class TriggerExtensions
    {
        public static TriggerKey GetTriggerKey(this ITrigger trigger)
        {
            return new TriggerKey(trigger.Name, trigger.Group);
        }
    }
}