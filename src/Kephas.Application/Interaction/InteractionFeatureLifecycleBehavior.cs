// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionFeatureLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the interaction feature lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Interaction;
    using Kephas.Services;

    /// <summary>
    /// An interaction feature lifecycle behavior notifying about <see cref="FeatureStartingEvent"/>, <see cref="FeatureStartedEvent"/>, <see cref="FeatureStoppingEvent"/>, and <see cref="FeatureStoppedEvent"/>.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class InteractionFeatureLifecycleBehavior : FeatureLifecycleBehaviorBase
    {
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionFeatureLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public InteractionFeatureLifecycleBehavior(IEventHub eventHub)
        {
            this.eventHub = eventHub;
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override Task BeforeInitializeAsync(IAppContext appContext, FeatureManagerMetadata serviceMetadata, CancellationToken cancellationToken = default)
        {
            return this.eventHub.PublishAsync(new FeatureStartingEvent(serviceMetadata.FeatureInfo), appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task AfterInitializeAsync(IAppContext appContext, FeatureManagerMetadata serviceMetadata, CancellationToken cancellationToken = default)
        {
            return this.eventHub.PublishAsync(new FeatureStartedEvent(serviceMetadata.FeatureInfo), appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called before a feature starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeFinalizeAsync(IAppContext appContext, FeatureManagerMetadata serviceMetadata, CancellationToken cancellationToken = default)
        {
            return this.eventHub.PublishAsync(new FeatureStoppingEvent(serviceMetadata.FeatureInfo), appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after a feature completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="serviceMetadata">The feature manager service metadata.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task AfterFinalizeAsync(IAppContext appContext, FeatureManagerMetadata serviceMetadata, CancellationToken cancellationToken = default)
        {
            return this.eventHub.PublishAsync(new FeatureStoppedEvent(serviceMetadata.FeatureInfo), appContext, cancellationToken);
        }
    }
}