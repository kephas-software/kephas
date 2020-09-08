// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelRunningJobEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the cancel running job event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    /// <summary>
    /// Event for cancelling a running job.
    /// </summary>
    public class CancelRunningJobEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the running job to be canceled.
        /// </summary>
        public object RunningJobId { get; set; }
    }
}