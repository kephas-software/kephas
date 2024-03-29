﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureLifecycleBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature lifecycle behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Base class for feature lifecycle behaviors.
    /// </summary>
    public abstract class FeatureLifecycleBehaviorBase : Loggable, IFeatureLifecycleBehavior
    {
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureLifecycleBehaviorBase"/> class.
        /// </summary>
        /// <param name="logger">The behavior logger.</param>
        protected FeatureLifecycleBehaviorBase(ILogger<FeatureLifecycleBehaviorBase> logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the feature initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> IFeatureLifecycleBehavior.BeforeInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.BeforeInitializeAsync(appContext, serviceMetadata, cancellationToken);
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the feature initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        public virtual Task<IOperationResult> BeforeInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> IFeatureLifecycleBehavior.AfterInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.AfterInitializeAsync(appContext, serviceMetadata, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        public virtual Task<IOperationResult> AfterInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> IFeatureLifecycleBehavior.BeforeFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.BeforeFinalizeAsync(appContext, serviceMetadata, cancellationToken);
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        public virtual Task<IOperationResult> BeforeFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> IFeatureLifecycleBehavior.AfterFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken)
        {
            this.EnsureInitialized(appContext);
            return this.AfterFinalizeAsync(appContext, serviceMetadata, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        public virtual Task<IOperationResult> AfterFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
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