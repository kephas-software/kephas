// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLifecycleBehaviorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application lifecycle behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for application lifecycle behaviors.
    /// </summary>
    public abstract class AppLifecycleBehaviorBase : IAppLifecycleBehavior
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
        public virtual Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}