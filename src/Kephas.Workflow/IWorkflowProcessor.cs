// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWorkflowProcessor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service for processing activities.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IWorkflowProcessor
    {
        /// <summary>
        /// Executes the activity asynchronously, enabling the activity behaviors.
        /// </summary>
        /// <remarks>
        /// The provided target and arguments may overwrite those set in the activity.
        /// </remarks>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the execute.
        /// </returns>
        Task<object?> ExecuteAsync(
            IActivity activity,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);
    }
}