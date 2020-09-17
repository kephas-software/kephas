// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableScheduledJobEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Event for enabling a scheduled job and the associated triggers.
    /// </summary>
    public class EnableScheduledJobEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the scheduled job.
        /// </summary>
        public object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled job instance.
        /// </summary>
        public IJobInfo? ScheduledJob { get; set; }
    }
}