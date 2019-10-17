// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalendarIntervalTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the calendar interval trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;

    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    using Kephas.Scheduling.Quartz.JobStore.Model;

    /// <summary>
    /// A calendar interval trigger.
    /// </summary>
    public class CalendarIntervalTrigger : Trigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarIntervalTrigger"/> class.
        /// </summary>
        public CalendarIntervalTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarIntervalTrigger"/> class.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="state">The state.</param>
        /// <param name="instanceName">Name of the instance.</param>
        public CalendarIntervalTrigger(ICalendarIntervalTrigger trigger, Model.TriggerState state, string instanceName)
            : base(trigger, state, instanceName)
        {
        }

        [BsonRepresentation(BsonType.String)]
        public IntervalUnit RepeatIntervalUnit { get; set; }

        public int RepeatInterval { get; set; }

        public int TimesTriggered { get; set; }

        public string TimeZone { get; set; }

        public bool PreserveHourOfDayAcrossDaylightSavings { get; set; }

        public bool SkipDayIfHourDoesNotExist { get; set; }

        public override void Initialize(global::Quartz.ITrigger trigger, Model.TriggerState state, string instanceName)
        {
            if (!(trigger is ICalendarIntervalTrigger calendarTrigger))
            {
                throw new ArgumentOutOfRangeException(nameof(trigger), $"Instance of type '{typeof(ICalendarIntervalTrigger)}' expected.");
            }

            base.Initialize(trigger, state, instanceName);
            this.RepeatIntervalUnit = calendarTrigger.RepeatIntervalUnit;
            this.RepeatInterval = calendarTrigger.RepeatInterval;
            this.TimesTriggered = calendarTrigger.TimesTriggered;
            this.TimeZone = calendarTrigger.TimeZone.Id;
            this.PreserveHourOfDayAcrossDaylightSavings = calendarTrigger.PreserveHourOfDayAcrossDaylightSavings;
            this.SkipDayIfHourDoesNotExist = calendarTrigger.SkipDayIfHourDoesNotExist;
        }

        public override global::Quartz.ITrigger GetTrigger()
        {
            var trigger = new CalendarIntervalTriggerImpl()
            {
                RepeatIntervalUnit = this.RepeatIntervalUnit,
                RepeatInterval = this.RepeatInterval,
                TimesTriggered = this.TimesTriggered,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone),
                PreserveHourOfDayAcrossDaylightSavings = this.PreserveHourOfDayAcrossDaylightSavings,
                SkipDayIfHourDoesNotExist = this.SkipDayIfHourDoesNotExist,
            };
            this.FillTrigger(trigger);
            return trigger;
        }
    }
}