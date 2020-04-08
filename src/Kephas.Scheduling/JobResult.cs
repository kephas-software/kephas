// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Scheduling
{
    using System.Threading.Tasks;

    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of a job.
    /// </summary>
    public class JobResult : OperationResult<object?>, IJobResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="executeTask">The execute task.</param>
        public JobResult(object job, Task<object?> executeTask)
            : base(executeTask)
        {
            this.Job = job;
        }

        /// <summary>
        /// Gets the job or its ID.
        /// </summary>
        /// <value>
        /// The job or its ID.
        /// </value>
        public object Job { get; }
    }
}
