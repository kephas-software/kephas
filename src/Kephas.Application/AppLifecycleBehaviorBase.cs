// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLifecycleBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application lifecycle behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application.Resources;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Base class for application lifecycle behaviors.
    /// </summary>
    public abstract class AppLifecycleBehaviorBase : Loggable, IAppLifecycleBehavior
    {
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLifecycleBehaviorBase"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected AppLifecycleBehaviorBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        Task IAppLifecycleBehavior.BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.BeforeAppInitializeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        public virtual Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task IAppLifecycleBehavior.AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.AfterAppInitializeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task IAppLifecycleBehavior.BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.BeforeAppFinalizeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task IAppLifecycleBehavior.AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.AfterAppFinalizeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IAppContext GetAppContext(IContext context)
        {
            return context is IAppContext appContext
                ? appContext
                : throw new ApplicationException(Strings.MismatchedAppContext_Exception.FormatWith(nameof(IAppContext)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureInitialized(IContext context)
        {
            if (!this.isInitialized)
            {
                this.Logger = this.GetLogger(context);
                this.isInitialized = true;
            }
        }
    }
}