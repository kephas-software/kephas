// --------------------------------------------------------------------------------------------------------------------
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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Composition;
    using Kephas.Logging;

    /// <summary>
    /// Base class for feature lifecycle behaviors.
    /// </summary>
    public abstract class FeatureLifecycleBehaviorBase : Loggable, IFeatureLifecycleBehavior
    {
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
        /// The asynchronous result.
        /// </returns>
        public virtual Task BeforeInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task AfterInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
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
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task BeforeFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task AfterFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }
}