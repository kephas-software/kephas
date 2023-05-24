// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Jobs
{
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Interface for job result.
    /// </summary>
    public interface IJobResult : IAsyncOperationResult
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
        /// Gets the logger for the job.
        /// </summary>
        ILogger? Logger { get; }

        /// <summary>
        /// Gets the identifier of the application instance running the job.
        /// </summary>
        /// <value>
        /// The identifier of the application instance running the job.
        /// </value>
        string? AppInstanceId { get; }

        /// <summary>
        /// Gets a value indicating whether cancellation is requested for the background job.
        /// </summary>
        /// <value>
        /// True if cancellation is requested for the background job, false if not.
        /// </value>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// Cancels the background job.
        /// </summary>
        /// <returns>An operation result to await.</returns>
        IAsyncOperationResult Cancel();
    }
}
