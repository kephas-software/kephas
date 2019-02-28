// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CronTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the cron trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    using Kephas.Scheduling.Quartz.JobStore.Model;

    internal class CronTrigger : Trigger
    {
        public CronTrigger()
        {
        }

        public CronTrigger(ICronTrigger trigger, Model.TriggerState state, string instanceName)
            : base(trigger, state, instanceName)
        {
            this.CronExpression = trigger.CronExpressionString;
            this.TimeZone = trigger.TimeZone.Id;
        }

        public string CronExpression { get; set; }

        public string TimeZone { get; set; }

        public override global::Quartz.ITrigger GetTrigger()
        {
            var trigger = new CronTriggerImpl()
            {
                CronExpressionString = this.CronExpression,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone)
            };
            this.FillTrigger(trigger);
            return trigger;
        }
    }
}