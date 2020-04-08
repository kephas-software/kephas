// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimerTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the timer trigger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Triggers
{
    using System;

    /// <summary>
    /// The timer trigger interface.
    /// </summary>
    public interface ITimerTrigger : ITrigger
    {
        /// <summary>
        /// Gets or sets the start time. If not set, the execution is triggered right away.
        /// </summary>
        DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the interval at which the execution should be triggered.
        /// </summary>
        TimeSpan? Interval { get; set; }

        /// <summary>
        /// Gets or sets the number of times the trigger will fire.
        /// </summary>
        int Count { get; set; }
    }
}