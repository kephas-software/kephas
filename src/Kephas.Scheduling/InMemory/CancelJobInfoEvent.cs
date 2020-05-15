// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelJobInfoEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Event for cancelling jobs and triggers based on a given job information.
    /// </summary>
    public class CancelJobInfoEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the job information.
        /// </summary>
        public object? JobInfoId { get; set; }

        /// <summary>
        /// Gets or sets the job information instance.
        /// </summary>
        public IJobInfo? JobInfo { get; set; }
    }
}