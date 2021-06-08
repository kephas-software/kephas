// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Formatting;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Encapsulates the result of a job.
    /// </summary>
    public class JobResult : OperationResult<object?>, IJobResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        public JobResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        /// <param name="runningJobId">The running job identifier.</param>
        /// <param name="jobOperation">The job operation.</param>
        public JobResult(object runningJobId, Task<object?> jobOperation)
            : base(jobOperation)
        {
            this.RunningJobId = runningJobId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        /// <param name="triggerId">The trigger identifier.</param>
        public JobResult(object triggerId)
        {
            this.TriggerId = triggerId;
        }

        /// <summary>
        /// Gets or sets the ID of the job information.
        /// </summary>
        public object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the job information.
        /// </summary>
        public IJobInfo? ScheduledJob { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the job.
        /// </summary>
        /// <value>
        /// The identifier of the job.
        /// </value>
        public object? RunningJobId { get; set; }

        /// <summary>
        /// Gets or sets the job.
        /// </summary>
        /// <value>
        /// The job.
        /// </value>
        public IJob? RunningJob { get; set; }

        /// <summary>
        /// Gets or sets the trigger identifier.
        /// </summary>
        public object? TriggerId { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token source.
        /// </summary>
        public CancellationTokenSource? CancellationTokenSource { get; set; }

        /// <summary>
        /// Gets or sets the logger for the job.
        /// </summary>
        public ILogger? Logger { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application instance running the job.
        /// </summary>
        /// <value>
        /// The identifier of the application instance running the job.
        /// </value>
        public string? AppInstanceId { get; set; }

        /// <summary>
        /// Gets a value indicating whether cancellation is requested for the background job.
        /// </summary>
        /// <value>
        /// True if cancellation is requested for the background job, false if not.
        /// </value>
        public bool IsCancellationRequested => this.CancellationTokenSource?.IsCancellationRequested ?? false;

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public override TimeSpan Elapsed
        {
            get
            {
                // if the job is not yet completed, the elapsed time is from the start time until now
                if (base.Elapsed == TimeSpan.Zero && this.StartedAt.HasValue)
                {
                        return this.EndedAt.HasValue
                            ? this.EndedAt.Value - this.StartedAt.Value
                            : DateTimeOffset.Now - this.StartedAt.Value;
                }

                return base.Elapsed;
            }
        }

        /// <summary>
        /// Converts this object to a serialization friendly representation.
        /// </summary>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        public override object ToData(IDataFormattingContext? context = null)
        {
            var expando = base.ToData(context).ToIndexable()!;
            expando[nameof(this.ScheduledJobId)] = this.ScheduledJobId;
            expando[nameof(this.RunningJobId)] = this.RunningJobId;
            expando[nameof(this.TriggerId)] = this.TriggerId;
            expando[nameof(this.StartedAt)] = this.StartedAt;
            expando[nameof(this.EndedAt)] = this.EndedAt;
            return expando;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.RunningJobId == null
                ? base.ToString()
                : $"[{this.RunningJobId}] {base.ToString()}";
        }

        /// <summary>
        /// Cancels the background job.
        /// </summary>
        /// <returns>An operation result to await.</returns>
        public virtual IOperationResult Cancel()
        {
            this.CancellationTokenSource?.Cancel();
            return true.ToOperationResult();
        }
    }
}