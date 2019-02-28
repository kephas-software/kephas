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

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    internal class CalendarIntervalTrigger : Trigger
    {
        public CalendarIntervalTrigger()
        {
        }

        public CalendarIntervalTrigger(ICalendarIntervalTrigger trigger, TriggerState state, string instanceName)
            : base(trigger, state, instanceName)
        {
            this.RepeatIntervalUnit = trigger.RepeatIntervalUnit;
            this.RepeatInterval = trigger.RepeatInterval;
            this.TimesTriggered = trigger.TimesTriggered;
            this.TimeZone = trigger.TimeZone.Id;
            this.PreserveHourOfDayAcrossDaylightSavings = trigger.PreserveHourOfDayAcrossDaylightSavings;
            this.SkipDayIfHourDoesNotExist = trigger.SkipDayIfHourDoesNotExist;
        }

        //TODO [BsonRepresentation(BsonType.String)]
        public IntervalUnit RepeatIntervalUnit { get; set; }

        public int RepeatInterval { get; set; }

        public int TimesTriggered { get; set; }

        public string TimeZone { get; set; }

        public bool PreserveHourOfDayAcrossDaylightSavings { get; set; }

        public bool SkipDayIfHourDoesNotExist { get; set; }

        public override ITrigger GetTrigger()
        {
            var trigger = new CalendarIntervalTriggerImpl()
            {
                RepeatIntervalUnit = this.RepeatIntervalUnit,
                RepeatInterval = this.RepeatInterval,
                TimesTriggered = this.TimesTriggered,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone),
                PreserveHourOfDayAcrossDaylightSavings = this.PreserveHourOfDayAcrossDaylightSavings,
                SkipDayIfHourDoesNotExist = this.SkipDayIfHourDoesNotExist
            };
            this.FillTrigger(trigger);
            return trigger;
        }
    }
}