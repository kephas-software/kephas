// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelJobEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the cancel job event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    /// <summary>
    /// Event for cancelling jobs.
    /// </summary>
    public class CancelJobEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the job to be canceled.
        /// </summary>
        public object JobId { get; set; }
    }
}