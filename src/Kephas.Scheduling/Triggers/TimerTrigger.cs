﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Threading;

    using Kephas.Services;

    /// <summary>
    /// A timer trigger.
    /// </summary>
    public class TimerTrigger : TriggerBase, ITimerTrigger
    {
        private Timer? timer;
        private int firedCount = 0;

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
        public TimeSpan? Interval { get; set; } = TimeSpan.FromDays(1);

        /// <summary>
        /// Gets or sets the number of times the trigger will fire.
        /// </summary>
        public int? Count { get; set; } = 1;

        /// <summary>
        /// Gets or sets the interval kind.
        /// </summary>
        /// <value>
        /// The interval kind.
        /// </value>
        public TimerIntervalKind IntervalKind { get; set; } = TimerIntervalKind.EndStart;

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        public override void Initialize(IContext? context = null)
        {
            base.Initialize(context);

            if ((this.Count.HasValue && this.Count <= 0) ||
                (this.EndTime.HasValue && this.EndTime <= DateTimeOffset.Now))
            {
                this.Dispose();
            }

            var startIn = this.StartTime.HasValue
                ? this.StartTime.Value - DateTimeOffset.Now
                : TimeSpan.Zero;
            if (startIn < TimeSpan.Zero)
            {
                startIn = TimeSpan.Zero;
            }

            var interval = this.GetNormalizedInterval();
            this.timer = new Timer(
                s => this.HandleTimer(),
                null,
                startIn,
                this.IntervalKind == TimerIntervalKind.EndStart ? Timeout.InfiniteTimeSpan : interval);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TriggerBase"/> and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.timer?.Dispose();
            this.timer = null;

            base.Dispose(disposing);
        }

        private void HandleTimer()
        {
            this.OnFire();

            this.firedCount++;
            if ((this.Count.HasValue && this.Count <= this.firedCount) ||
                (this.EndTime.HasValue && this.EndTime.Value <= DateTimeOffset.Now) ||
                this.GetNormalizedInterval() == Timeout.InfiniteTimeSpan)
            {
                this.Dispose();
            }
            else if (this.IntervalKind == TimerIntervalKind.EndStart)
            {
                var startIn = this.GetNormalizedInterval();
                this.timer?.Dispose();
                this.timer = new Timer(
                    s => this.HandleTimer(),
                    null,
                    startIn,
                    Timeout.InfiniteTimeSpan);
            }
        }

        private TimeSpan GetNormalizedInterval()
        {
            return this.Interval.HasValue && this.Interval.Value > TimeSpan.Zero
                            ? this.Interval.Value
                            : Timeout.InfiniteTimeSpan;
        }
    }
}