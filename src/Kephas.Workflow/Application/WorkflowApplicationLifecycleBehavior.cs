// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the workflow application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Workflow.Runtime;

    /// <summary>
    /// A workflow application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class WorkflowApplicationLifecycleBehavior : AppLifecycleBehaviorBase
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            RuntimeTypeInfo.RegisterFactory(new WorkflowTypeInfoFactory());

            return base.BeforeAppInitializeAsync(appContext, cancellationToken);
        }
    }
}