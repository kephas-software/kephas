// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Scheduling.Reflection
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Workflow;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Interface for job information.
    /// </summary>
    public interface IJobInfo : IActivityInfo
    {
        /// <summary>
        /// Gets the job triggers.
        /// </summary>
        /// <value>
        /// The job triggers.
        /// </value>
        IEnumerable<ITrigger> Triggers { get; }

        /// <summary>
        /// Executes the job asynchronously.
        /// </summary>
        /// <param name="job">The job to execute.</param>
        /// <param name="target">The job target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        Task<object?> ExecuteAsync(
            IJob job,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default);
    }
}