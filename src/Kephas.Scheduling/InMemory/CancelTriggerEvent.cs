// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelTriggerEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the cancel trigger event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    /// <summary>
    /// Event for cancelling triggers.
    /// </summary>
    public class CancelTriggerEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the trigger to be canceled.
        /// </summary>
        public object TriggerId { get; set; }
    }
}