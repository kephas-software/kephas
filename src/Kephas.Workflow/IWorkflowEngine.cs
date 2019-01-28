// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWorkflowEngine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Shared application service for executing activities.
    /// </summary>
    [SharedAppServiceContract]
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Executes the task asynchronously.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a result.
        /// </returns>
        Task<IExpando> ExecuteAsync(
            IActivity activity,
            IActivityContext context,
            CancellationToken cancellationToken = default);
    }
}