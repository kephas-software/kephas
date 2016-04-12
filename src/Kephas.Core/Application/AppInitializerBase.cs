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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for application initializers providing an initialization monitor.
    /// </summary>
    public abstract class AppInitializerBase : IAppInitializer
    {
#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// The initialization monitor.
        /// </summary>
        protected readonly InitializationMonitor<IAppInitializer> InitializationMonitor;
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInitializerBase"/> class.
        /// </summary>
        protected AppInitializerBase()
        {
            this.InitializationMonitor = new InitializationMonitor<IAppInitializer>(this.GetType());
        }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.InitializationMonitor.Start();

            try
            {
                await this.InitializeCoreAsync(appContext, cancellationToken).WithServerThreadingContext();
                this.InitializationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.InitializationMonitor.Fault(ex);
                throw;
            }
        }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected abstract Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken);
    }
}