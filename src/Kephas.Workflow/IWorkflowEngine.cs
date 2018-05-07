// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowEngine.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// <param name="taskInfo">The task information.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of an <see cref="IExecutionResult"/>.
        /// </returns>
        Task<IExpando> ExecuteAsync(
            IActivity activity,
            IWorkflowExecutionContext context,
            CancellationToken cancellationToken = default);    }
}