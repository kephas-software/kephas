// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureManagerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

    using Kephas.Services;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for application initializers providing an initialization monitor.
    /// </summary>
    public abstract class FeatureManagerBase : IFeatureManager
    {
#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// The initialization monitor.
        /// </summary>
        protected readonly InitializationMonitor<IFeatureManager> InitializationMonitor;

        /// <summary>
        /// The finalization monitor.
        /// </summary>
        protected readonly FinalizationMonitor<IFeatureManager> FinalizationMonitor;
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureManagerBase"/> class.
        /// </summary>
        protected FeatureManagerBase()
        {
            this.InitializationMonitor = new InitializationMonitor<IFeatureManager>(this.GetType());
            this.FinalizationMonitor = new FinalizationMonitor<IFeatureManager>(this.GetType());
        }

        /// <summary>
        /// Initializes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.InitializationMonitor.Start();

            try
            {
                await this.InitializeCoreAsync(appContext, cancellationToken).PreserveThreadContext();
                this.InitializationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.InitializationMonitor.Fault(ex);
                throw;
            }
        }

        /// <summary>
        /// Finalizes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task FinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.FinalizationMonitor.Start();

            try
            {
                await this.FinalizeCoreAsync(appContext, cancellationToken).PreserveThreadContext();
                this.FinalizationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.FinalizationMonitor.Fault(ex);
                throw;
            }
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        Task IAsyncInitializable.InitializeAsync(IContext context, CancellationToken cancellationToken)
        {
            return this.InitializeAsync((IAppContext)context, cancellationToken);
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task IAsyncFinalizable.FinalizeAsync(IContext context, CancellationToken cancellationToken)
        {
            return this.FinalizeAsync((IAppContext)context, cancellationToken);
        }

        /// <summary>
        /// Initializes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Finalizes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}