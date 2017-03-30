// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppFinalizerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application finalizer base class.
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
    /// Base class for application finalizers, providing an initialization monitor.
    /// </summary>
    public abstract class AppFinalizerBase : IAppFinalizer
    {
#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// The initialization monitor.
        /// </summary>
        protected readonly InitializationMonitor<IAppFinalizer> InitializationMonitor;
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFinalizerBase"/> class.
        /// </summary>
        protected AppFinalizerBase()
        {
            this.InitializationMonitor = new InitializationMonitor<IAppFinalizer>(this.GetType());
        }

        /// <summary>
        /// Finalizes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task FinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = new CancellationToken())
        {
            this.InitializationMonitor.Start();

            try
            {
                await this.FinalizeCoreAsync(appContext, cancellationToken).PreserveThreadContext();
                this.InitializationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.InitializationMonitor.Fault(ex);
                throw;
            }
        }

        /// <summary>
        /// Finalizes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected abstract Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken);
    }
}