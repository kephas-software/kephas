// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobSchedulerClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobSchedulerClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System.Threading;

    using Kephas.Dynamic;
    using Kephas.Scheduling.Reflection;
    using Kephas.Services;
    using Kephas.Workflow;

    /// <summary>
    /// Interface for job scheduler client.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IJobSchedulerClient
    {
        /// <summary>
        /// Starts a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a <see cref="IJobInfo"/>.
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
        IJobResult StartJobAsync(
            object jobInfo,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default);
    }
}
