// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableScheduledJobMessage.cs" company="Kephas Software SRL">
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
    /// Message for enabling a scheduled job.
    /// </summary>
    [Display(Description = "Enables the specified scheduled job.")]
    public class EnableScheduledJobMessage : IMessage<Response>
    {
        /// <summary>
        /// Gets or sets the ID of the scheduled job to be enabled.
        /// </summary>
        [Display(Description = "The job or the ID of the scheduled job to be enabled.")]
        public object? Job { get; set; }
    }
}