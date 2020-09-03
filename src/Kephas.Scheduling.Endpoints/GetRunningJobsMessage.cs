// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetRunningJobsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;

    /// <summary>
    /// Message for getting the running jobs.
    /// </summary>
    [DisplayInfo(Description = "Gets the running jobs.")]
    public class GetRunningJobsMessage : IMessage
    {
    }

    /// <summary>
    /// Response message for <see cref="GetRunningJobsMessage"/>.
    /// </summary>
    public class GetRunningJobsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the jobs data.
        /// </summary>
        public RunningJobData[] Jobs { get; set; }
    }

    /// <summary>
    /// Data for a running job.
    /// </summary>
    public class RunningJobData
    {
        /// <summary>
        /// Gets or sets the job ID.
        /// </summary>
        public object JobId { get; set; }

        /// <summary>
        /// Gets or sets the completion percentage.
        /// </summary>
        public float PercentCompleted { get; set; }

        /// <summary>
        /// Gets or sets the operation state.
        /// </summary>
        public OperationState OperationState { get; set; }

        /// <summary>
        /// Gets or sets the time when the job started.
        /// </summary>
        public DateTimeOffset? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        public TimeSpan Elapsed { get; set; }

        /// <summary>
        /// Gets or sets the job information.
        /// </summary>
        public object? JobInfo { get; set; }

        /// <summary>
        /// Gets or sets the ID of the job information.
        /// </summary>
        public object? JobInfoId { get; set; }
    }
}