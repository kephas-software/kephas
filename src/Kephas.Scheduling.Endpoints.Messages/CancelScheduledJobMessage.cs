﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelScheduledJobMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Message for cancelling a scheduled job.
    /// </summary>
    [DisplayInfo(Description = "Cancels the scheduled job with the given ID.")]
    public class CancelScheduledJobMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the identifier of the scheduled job.
        /// </summary>
        [DisplayInfo(Description = "The ID of the scheduled job.")]
        public object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled job instance.
        /// </summary>
        public IJobInfo? ScheduledJob { get; set; }
    }

    /// <summary>
    /// Response for <see cref="CancelScheduledJobMessage"/>.
    /// </summary>
    public class CancelScheduledJobResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the result of the cancel scheduled job operation.
        /// </summary>
        public IOperationResult<IJobInfo?>? Result { get; set; }
    }
}