// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Logging;

namespace Kephas.Scheduling.Jobs
{
    using System;
    using System.Threading;

    using Kephas.Data;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Interface for job result.
    /// </summary>
    public interface IJobResult : IOperationResult
    {
        /// <summary>
        /// Gets the identifier of the scheduled job.
        /// </summary>
        /// <value>
        /// The identifier of the scheduled job.
        /// </value>
        object? ScheduledJobId { get; }

        /// <summary>
        /// Gets the job information.
        /// </summary>
        /// <value>
        /// The job information.
        /// </value>
        IJobInfo? ScheduledJob { get; }

        /// <summary>
        /// Gets the identifier of the running job.
        /// </summary>
        /// <value>
        /// The identifier of the running job.
        /// </value>
        object? RunningJobId { get; }

        /// <summary>
        /// Gets the running job.
        /// </summary>
        /// <value>
        /// The running job.
        /// </value>
        IJob? RunningJob { get; }

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

        /// <summary>
        /// Gets the cancellation token source.
        /// </summary>
        CancellationTokenSource? CancellationTokenSource { get; }

        /// <summary>
        /// Gets the logger for the job.
        /// </summary>
        ILogger? Logger { get; }
    }
}
