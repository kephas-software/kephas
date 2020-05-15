// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Scheduling.Jobs
{
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Interface for job result.
    /// </summary>
    public interface IJobResult : IOperationResult
    {
        /// <summary>
        /// Gets the identifier of the job.
        /// </summary>
        /// <value>
        /// The identifier of the job.
        /// </value>
        object? JobInfoId { get; }

        /// <summary>
        /// Gets the job information.
        /// </summary>
        /// <value>
        /// The job information.
        /// </value>
        IJobInfo? JobInfo { get; }

        /// <summary>
        /// Gets the identifier of the job.
        /// </summary>
        /// <value>
        /// The identifier of the job.
        /// </value>
        object? JobId { get; }

        /// <summary>
        /// Gets the job.
        /// </summary>
        /// <value>
        /// The job.
        /// </value>
        IJob? Job { get; }

        /// <summary>
        /// Gets the identifier of the trigger.
        /// </summary>
        /// <value>
        /// The identifier of the trigger.
        /// </value>
        object? TriggerId { get; }

        /// <summary>
        /// Gets the time when the job started.
        /// </summary>
        DateTimeOffset? StartedAt { get; }

        /// <summary>
        /// Gets the time when the job ended.
        /// </summary>
        DateTimeOffset? EndedAt { get; }
    }
}
