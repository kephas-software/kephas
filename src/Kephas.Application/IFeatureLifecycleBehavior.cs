// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFeatureLifecycleBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Singleton service contract for feature lifecycle behaviors.
    /// </summary>
    /// <remarks>
    /// A feature lifecycle behavior intercepts the initialization and finalization of one or more features
    /// and reacts to them. Such features could log the performance of initialization,
    /// check prerequisites like proper licensing or whatever the application needs.
    /// </remarks>
    [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(TargetFeatureAttribute) })]
    public interface IFeatureLifecycleBehavior
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
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> BeforeInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> AfterInitializeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default);

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
        Task<IOperationResult> BeforeFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        Task<IOperationResult> AfterFinalizeAsync(
            IAppContext appContext,
            FeatureManagerMetadata serviceMetadata,
            CancellationToken cancellationToken = default);
    }
}