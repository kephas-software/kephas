// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelRunningJobMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;

    /// <summary>
    /// Message for canceling running jobs.
    /// </summary>
    [Display(Description = "Cancels the running job with the given ID.")]
    public class CancelRunningJobMessage : IMessage<CancelRunningJobResponse>
    {
        /// <summary>
        /// Gets or sets the identifier of the running job to be canceled.
        /// </summary>
        [Display(Description = "The ID of the running job.")]
        public object? RunningJobId { get; set; }

        /// <summary>
        /// Gets or sets the running job to be canceled.
        /// </summary>
        public IJobResult? RunningJob { get; set; }
    }

    /// <summary>
    /// Response for <see cref="CancelRunningJobMessage"/>.
    /// </summary>
    public class CancelRunningJobResponse : Response
    {
        /// <summary>
        /// Gets or sets the result of the cancel running job operation.
        /// </summary>
        public IOperationResult<IJobResult?>? Result { get; set; }
    }
}