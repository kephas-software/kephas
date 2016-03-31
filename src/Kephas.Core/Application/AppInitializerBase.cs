// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInitializerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application initializer base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for application initializers.
    /// </summary>
    public abstract class AppInitializerBase : IAppInitializer
    {
        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CompletedTask.Value;
        }

        /// <summary>
        /// Initializes the application asynchronously. The call can be executed with a delay, it is not critical for the application startup.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task InitializeDelayedAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CompletedTask.Value;
        }
    }
}