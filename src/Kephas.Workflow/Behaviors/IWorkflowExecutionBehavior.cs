// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowExecutionBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWorkflowExecutionBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for workflow execution behavior.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true)]
    public interface IWorkflowExecutionBehavior
    {
        /// <summary>
        /// Interception called before invoking the service to execute the activity.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        Task BeforeExecuteAsync(IWorkflowExecutionContext context, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the service to execute the activity.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        /// <remarks>
        /// The context will contain the response returned by the service.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        Task AfterExecuteAsync(IWorkflowExecutionContext context, CancellationToken token);
    }
}