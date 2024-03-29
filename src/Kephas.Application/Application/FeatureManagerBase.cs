﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application.Resources;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for application initializers providing an initialization monitor.
    /// </summary>
    public abstract class FeatureManagerBase : Loggable, IFeatureManager
    {
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureManagerBase"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected FeatureManagerBase(ILogManager? logManager = null)
            : base(logManager)
        {
            this.InitializationMonitor = new InitializationMonitor<IFeatureManager>(this.GetType());
            this.FinalizationMonitor = new FinalizationMonitor<IFeatureManager>(this.GetType());
        }

        /// <summary>
        /// Gets the initialization monitor.
        /// </summary>
        protected InitializationMonitor<IFeatureManager> InitializationMonitor { get; }

        /// <summary>
        /// Gets the finalization monitor.
        /// </summary>
        protected FinalizationMonitor<IFeatureManager> FinalizationMonitor { get; }

        /// <summary>
        /// Initializes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task InitializeAsync(IAppContext? appContext, CancellationToken cancellationToken = default)
        {
            appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));

            this.EnsureLoggerInitialized(appContext);

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
        public async Task FinalizeAsync(IAppContext? appContext, CancellationToken cancellationToken = default)
        {
            appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));

            this.EnsureLoggerInitialized(appContext);

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
        /// Initializes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureLoggerInitialized(IContext context)
        {
            if (!this.isInitialized)
            {
                this.Logger = this.GetLogger(context);
                this.isInitialized = true;
            }
        }
    }
}