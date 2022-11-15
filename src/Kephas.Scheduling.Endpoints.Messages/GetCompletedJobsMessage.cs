// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCompletedJobsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Message for getting the completed jobs.
    /// </summary>
    [Display(Description = "Gets the completed jobs.")]
    public class GetCompletedJobsMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the number of packages to skip.
        /// </summary>
        /// <value>
        /// The number of packages to skip.
        /// </value>
        [Display(Description = "Optional. Value indicating the number of jobs to skip (default: 0).")]
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of packages to take.
        /// </summary>
        /// <value>
        /// The number of packages to take.
        /// </value>
        [Display(Description = "Optional. Value indicating the number of jobs to take (default: 20).")]
        public int Take { get; set; } = 20;
    }

    /// <summary>
    /// Response message for getting the completed jobs.
    /// </summary>
    public class GetCompletedJobsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the jobs data.
        /// </summary>
        public CompletedJobData[] Jobs { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int? TotalCount { get; set; }
    }

    /// <summary>
    /// Provides data fro completed jobs.
    /// </summary>
    public class CompletedJobData : RunningJobData
    {
        /// <summary>
        /// Gets or sets the time when the job ended.
        /// </summary>
        public DateTimeOffset? EndedAt { get; set; }

        /// <summary>
        /// Gets or sets the return value of the executed task.
        /// </summary>
        public object? ReturnValue { get; set; }
    }
}