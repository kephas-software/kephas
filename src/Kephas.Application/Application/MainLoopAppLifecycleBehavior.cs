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
    /// An application lifecycle behavior initializing the main loop.
    /// </summary>
    /// <remarks>
    /// Makes sure that the application main loop has a chance to initialize before the application manager starts initializing the features.
    /// Sometimes the features and the behaviors order a shutdown (like a setup routine).
    /// </remarks>
    [ProcessingPriority(Priority.Highest)]
    public class MainLoopAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IAppMainLoop mainLoop;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainLoopAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="mainLoop">The application's main loop.</param>
        /// <param name="appContext">The application context.</param>
        public MainLoopAppLifecycleBehavior(IAppMainLoop mainLoop, IAppContext appContext)
        {
            this.mainLoop = mainLoop ?? throw new ArgumentNullException(nameof(mainLoop));
            this.appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> BeforeAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            await ServiceHelper.InitializeAsync(this.mainLoop, this.appContext, cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> AfterAppFinalizeAsync(CancellationToken cancellationToken = default)
        {
            await ServiceHelper.FinalizeAsync(this.mainLoop, this.appContext, cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }
    }
}
