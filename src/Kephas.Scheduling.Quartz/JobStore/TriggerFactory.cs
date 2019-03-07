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

    /// <summary>
    /// A trigger factory.
    /// </summary>
    internal static class TriggerFactory
    {
        /// <summary>
        /// Creates a trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="state">The state.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <returns>
        /// The new trigger.
        /// </returns>
        public static Trigger CreateTrigger(ITrigger trigger, Model.TriggerState state, string instanceName)
        {
            switch (trigger)
            {
                case ICronTrigger cronTrigger:
                    return new CronTrigger(cronTrigger, state, instanceName);
                case ISimpleTrigger simpleTrigger:
                    return new SimpleTrigger(simpleTrigger, state, instanceName);
                case ICalendarIntervalTrigger intervalTrigger:
                    return new CalendarIntervalTrigger(intervalTrigger, state, instanceName);
                case IDailyTimeIntervalTrigger timeIntervalTrigger:
                    return new DailyTimeIntervalTrigger(timeIntervalTrigger, state, instanceName);
                default:
                    throw new NotSupportedException($"Trigger of type {trigger.GetType().FullName} is not supported");
            }
        }
    }
}