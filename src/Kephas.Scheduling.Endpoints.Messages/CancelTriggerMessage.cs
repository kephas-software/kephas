﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelTriggerMessage.cs" company="Kephas Software SRL">
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
    using Kephas.Scheduling.Triggers;

    /// <summary>
    /// Message for canceling a trigger.
    /// </summary>
    [Display(Description = "Cancels the trigger with the given ID.")]
    public class CancelTriggerMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the identifier of the trigger to be canceled.
        /// </summary>
        [Display(Description = "The ID of the trigger to be canceled.")]
        public object? TriggerId { get; set; }

        /// <summary>
        /// Gets or sets the trigger to be canceled.
        /// </summary>
        public ITrigger? Trigger { get; set; }
    }

    /// <summary>
    /// Response for <see cref="CancelTriggerResponseMessage"/>.
    /// </summary>
    public class CancelTriggerResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the result of the cancel trigger operation.
        /// </summary>
        public IOperationResult<ITrigger?>? Result { get; set; }
    }
}