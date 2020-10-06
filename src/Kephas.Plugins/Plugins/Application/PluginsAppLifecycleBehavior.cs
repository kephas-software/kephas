// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Interaction;
    using Kephas.Interaction;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Application lifecycle behavior for plugins.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class PluginsAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;
        private IEventSubscription? setupQuerySubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The app runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        public PluginsAppLifecycleBehavior(IAppRuntime appRuntime, IEventHub eventHub)
        {
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.setupQuerySubscription = this.eventHub.Subscribe<AppSetupQueryEvent>((e, c) => e.SetupEnabled = e.SetupEnabled && !this.appRuntime.PluginsEnabled());
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> AfterAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
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
        public Task<IOperationResult> BeforeAppFinalizeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> AfterAppFinalizeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.setupQuerySubscription?.Dispose();
            this.setupQuerySubscription = null;

            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}
