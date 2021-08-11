// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutdownAwaiterAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the shutdown awaiter application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A shutdown awaiter application lifecycle behavior.
    /// </summary>
    /// <remarks>
    /// Makes sure that the shutdown awaiter has a chance to initialize before the application manager starts initializing the features.
    /// Sometimes the features and the behaviors order a shutdown (like a setup routine).
    /// </remarks>
    [ProcessingPriority(Priority.Highest)]
    public class ShutdownAwaiterAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppMainLoop awaiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownAwaiterAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="awaiter">The awaiter.</param>
        public ShutdownAwaiterAppLifecycleBehavior(IAppMainLoop awaiter)
        {
            this.awaiter = awaiter;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override async Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            await ServiceHelper.InitializeAsync(this.awaiter, appContext, cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override async Task<IOperationResult> AfterAppFinalizeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            await ServiceHelper.FinalizeAsync(this.awaiter, appContext, cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }
    }
}
