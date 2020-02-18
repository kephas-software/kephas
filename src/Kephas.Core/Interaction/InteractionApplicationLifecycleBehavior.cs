// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionApplicationLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the interaction application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An interaction application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class InteractionApplicationLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionApplicationLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public InteractionApplicationLifecycleBehavior(IEventHub eventHub)
        {
            this.eventHub = eventHub;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return ServiceHelper.InitializeAsync(this.eventHub, appContext);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return ServiceHelper.FinalizeAsync(this.eventHub, appContext);
        }
    }
}
