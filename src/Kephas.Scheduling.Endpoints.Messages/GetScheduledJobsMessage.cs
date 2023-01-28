// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetScheduledJobsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Message for getting the scheduled jobs.
    /// </summary>
    [Display(Description = "Gets the scheduled jobs.")]
    public class GetScheduledJobsMessage : IMessage
    {
    }

    /// <summary>
    /// Response message for <see cref="GetScheduledJobsMessage"/>.
    /// </summary>
    public class GetScheduledJobsResponse : Response
    {
        /// <summary>
        /// Gets or sets the scheduled jobs.
        /// </summary>
        public ScheduledJobData[] Jobs { get; set; }
    }

    /// <summary>
    /// Data for a scheduled job.
    /// </summary>
    public class ScheduledJobData
    {
        /// <summary>
        /// Gets or sets the job info ID.
        /// </summary>
        public object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the job information.
        /// </summary>
        public object? ScheduledJob { get; set; }

        /// <summary>
        /// Gets or sets the triggers.
        /// </summary>
        public string[] Triggers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the scheduled job is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}