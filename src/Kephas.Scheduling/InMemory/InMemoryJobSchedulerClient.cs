// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryJobSchedulerClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory job scheduler client class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;
    using Kephas.Services;
    using Kephas.Workflow;

    /// <summary>
    /// An in memory job scheduler client.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InMemoryJobSchedulerClient : IJobSchedulerClient
    {
        /// <summary>
        /// Starts a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a
        /// <see cref="IJobInfo"/>.
        /// </para>
        /// </summary>
        /// <param name="jobInfo">Information describing the job.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public IJobResult StartJobAsync(
            object jobInfo,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            if (!(jobInfo is IJobInfo jobInfoObject))
            {
                throw new ArgumentException($"Only {nameof(IJobInfo)} values accepted for the {nameof(jobInfo)} parameter.", nameof(jobInfo));
            }

            var job = (IJob)jobInfoObject.CreateInstance();
            job.Target = target;
            job.Arguments = arguments;
            job.Context = context;

            return new JobResult(jobInfoObject, jobInfoObject.ExecuteAsync(job, target, arguments, context, cancellationToken));
        }
    }
}
