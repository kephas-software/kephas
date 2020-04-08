// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the timer trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Triggers
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A timer trigger.
    /// </summary>
    public class TimerTrigger : TriggerBase, ITimerTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerTrigger"/> class.
        /// </summary>
        public TimerTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerTrigger"/> class.
        /// </summary>
        /// <param name="id">The trigger ID.</param>
        public TimerTrigger(object id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the start time. If not set, the execution is triggered right away.
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the interval at which the execution should be triggered.
        /// </summary>
        public TimeSpan? Interval { get; set; }

        /// <summary>
        /// Gets or sets the number of times the trigger will fire.
        /// </summary>
        public int Count { get; set; } = 1;

        public override void Initialize(IContext? context = null)
        {
            base.Initialize(context);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}