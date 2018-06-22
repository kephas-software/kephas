// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration feature manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;

    /// <summary>
    /// Manager for orchestration features.
    /// </summary>
    [FeatureInfo(FeatureName, isRequired: true)]
    public class OrchestrationFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Name of the orchestration feature.
        /// </summary>
        public const string FeatureName = "Orchestration";

        /// <summary>
        /// Manager for orchestration.
        /// </summary>
        private readonly IOrchestrationManager orchestrationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationFeatureManager"/> class.
        /// </summary>
        /// <param name="orchestrationManager">Manager for orchestration.</param>
        public OrchestrationFeatureManager(IOrchestrationManager orchestrationManager)
        {
            this.orchestrationManager = orchestrationManager;
        }

        /// <summary>Initializes the feature asynchronously.</summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return this.orchestrationManager.InitializeAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Finalizes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return this.orchestrationManager.FinalizeAsync(appContext, cancellationToken);
        }
    }
}