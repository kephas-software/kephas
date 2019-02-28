// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the simple trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    using Kephas.Scheduling.Quartz.JobStore.Model;

    /// <summary>
    /// A simple trigger.
    /// </summary>
    public class SimpleTrigger : Trigger
    {
        public SimpleTrigger()
        {
        }

        public SimpleTrigger(ISimpleTrigger trigger, Model.TriggerState state, string instanceName)
            : base(trigger, state, instanceName)
        {
            this.RepeatCount = trigger.RepeatCount;
            this.RepeatInterval = trigger.RepeatInterval;
            this.TimesTriggered = trigger.TimesTriggered;
        }

        public int RepeatCount { get; set; }

        public TimeSpan RepeatInterval { get; set; }

        public int TimesTriggered { get; set; }

        public override global::Quartz.ITrigger GetTrigger()
        {
            var trigger = new SimpleTriggerImpl()
            {
                RepeatCount = this.RepeatCount,
                RepeatInterval = this.RepeatInterval,
                TimesTriggered = this.TimesTriggered
            };
            this.FillTrigger(trigger);
            return trigger;
        }
    }
}