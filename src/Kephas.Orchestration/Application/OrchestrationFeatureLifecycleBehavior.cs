// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationFeatureBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration feature lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Interaction;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Services;

    /// <summary>
    /// An orchestration feature lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class OrchestrationFeatureLifecycleBehavior : FeatureLifecycleBehaviorBase
    {
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationFeatureLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public OrchestrationFeatureLifecycleBehavior(IEventHub eventHub)
        {
            this.eventHub = eventHub;
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
            return this.eventHub.PublishAsync(new FeatureStartedEvent { Feature = serviceMetadata.FeatureInfo }, appContext, cancellationToken);
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
            return this.eventHub.PublishAsync(new FeatureStoppedEvent { Feature = serviceMetadata.FeatureInfo }, appContext, cancellationToken);
        }
    }
}