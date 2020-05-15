// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetScheduledJobsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Messaging.Messages;

namespace Kephas.Scheduling.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// Message for getting the scheduled jobs.
    /// </summary>
    [DisplayInfo(Description = "Gets the scheduled jobs.")]
    public class GetScheduledJobsMessage : IMessage
    {
    }

    /// <summary>
    /// Response message for <see cref="GetScheduledJobsMessage"/>.
    /// </summary>
    public class GetScheduledJobsResponseMessage : ResponseMessage
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
        /// Gets or sets the job name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the triggers.
        /// </summary>
        public string[] Triggers { get; set; }
    }
}