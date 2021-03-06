﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableScheduledJobMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// Message for disabling a scheduled job.
    /// </summary>
    [DisplayInfo(Description = "Disables the specified scheduled job.")]
    public class DisableScheduledJobMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the ID of the scheduled job to be disabled.
        /// </summary>
        [DisplayInfo(Description = "The job or the ID of the scheduled job to be disabled.")]
        public object? Job { get; set; }
    }
}