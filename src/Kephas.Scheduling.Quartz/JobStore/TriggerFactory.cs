// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the trigger factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;

    using global::Quartz;

    using Kephas.Scheduling.Quartz.JobStore.Models;

    internal static class TriggerFactory
    {
        public static Trigger CreateTrigger(ITrigger trigger, Model.TriggerState state, string instanceName)
        {
            if (trigger is ICronTrigger)
            {
                return new CronTrigger((ICronTrigger) trigger, state, instanceName);
            }
            if (trigger is ISimpleTrigger)
            {
                return new SimpleTrigger((ISimpleTrigger) trigger, state, instanceName);
            }
            if (trigger is ICalendarIntervalTrigger)
            {
                return new CalendarIntervalTrigger((ICalendarIntervalTrigger) trigger, state, instanceName);
            }
            if (trigger is IDailyTimeIntervalTrigger)
            {
                return new DailyTimeIntervalTrigger((IDailyTimeIntervalTrigger) trigger, state, instanceName);
            }

            throw new NotSupportedException($"Trigger of type {trigger.GetType().FullName} is not supported");
        }
    }}