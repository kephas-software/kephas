// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationAppLifecycleBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// An orchestration application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class OrchestrationAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        /// <summary>
        /// Manager for orchestration.
        /// </summary>
        private readonly IOrchestrationManager orchestrationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="orchestrationManager">Manager for orchestration.</param>
        public OrchestrationAppLifecycleBehavior(
            IOrchestrationManager orchestrationManager)
        {
            Requires.NotNull(orchestrationManager, nameof(orchestrationManager));

            this.orchestrationManager = orchestrationManager;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<OrchestrationAppLifecycleBehavior> Logger { get; set; }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>A Task.</returns>
        public override async Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            // TODO notify the orchestration manager about the completion of the app start
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override async Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            // TODO notify the orchestration manager about the completion of the app stop
        }
    }
}