// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DailyTimeIntervalTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the daily time interval trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;
    using System.Collections.Generic;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    using Kephas.Scheduling.Quartz.JobStore.Model;

    internal class DailyTimeIntervalTrigger : Trigger
    {
        public DailyTimeIntervalTrigger()
        {
        }

        public DailyTimeIntervalTrigger(IDailyTimeIntervalTrigger trigger, Model.TriggerState state, string instanceName)
            : base(trigger, state, instanceName)
        {
        }

        public int RepeatCount { get; set; }

        //TODO [BsonRepresentation(BsonType.String)]
        public IntervalUnit RepeatIntervalUnit { get; set; }

        public int RepeatInterval { get; set; }

        public TimeOfDay StartTimeOfDay { get; set; }

        public TimeOfDay EndTimeOfDay { get; set; }

        public HashSet<DayOfWeek> DaysOfWeek { get; set; }

        public int TimesTriggered { get; set; }

        public string TimeZone { get; set; }

        public override void Initialize(global::Quartz.ITrigger trigger, Model.TriggerState state, string instanceName)
        {
            if (!(trigger is IDailyTimeIntervalTrigger dailyTrigger))
            {
                throw new ArgumentOutOfRangeException(nameof(trigger), $"Instance of type '{typeof(IDailyTimeIntervalTrigger)}' expected.");
            }

            base.Initialize(trigger, state, instanceName);
            this.RepeatCount = dailyTrigger.RepeatCount;
            this.RepeatIntervalUnit = dailyTrigger.RepeatIntervalUnit;
            this.RepeatInterval = dailyTrigger.RepeatInterval;
            this.StartTimeOfDay = dailyTrigger.StartTimeOfDay;
            this.EndTimeOfDay = dailyTrigger.EndTimeOfDay;
            this.DaysOfWeek = new HashSet<DayOfWeek>(dailyTrigger.DaysOfWeek);
            this.TimesTriggered = dailyTrigger.TimesTriggered;
            this.TimeZone = dailyTrigger.TimeZone.Id;
        }

        public override global::Quartz.ITrigger GetTrigger()
        {
            var trigger = new DailyTimeIntervalTriggerImpl
            {
                RepeatCount = this.RepeatCount,
                RepeatIntervalUnit = this.RepeatIntervalUnit,
                RepeatInterval = this.RepeatInterval,
                StartTimeOfDay = this.StartTimeOfDay ?? new TimeOfDay(0, 0, 0),
                EndTimeOfDay = this.EndTimeOfDay ?? new TimeOfDay(23, 59, 59),
                DaysOfWeek = (IReadOnlyCollection<DayOfWeek>)new HashSet<DayOfWeek>(this.DaysOfWeek),
                TimesTriggered = this.TimesTriggered,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone)
            };
            this.FillTrigger(trigger);
            return trigger;
        }
    }
}