// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Interaction;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Plugins;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for plugin handlers.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public abstract class PluginHandlerBase<TMessage, TResponse> : MessageHandlerBase<TMessage, TResponse>
        where TMessage : class
        where TResponse : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginHandlerBase{TMessage, TResponse}"/> class.
        /// </summary>
        /// <param name="pluginManager">Gets the plugin manager.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected PluginHandlerBase(
            IPluginManager pluginManager,
            IEventHub eventHub,
            ILogger<PluginHandlerBase<TMessage, TResponse>>? logger = null)
            : base(logger)
        {
            this.PluginManager = pluginManager;
            this.EventHub = eventHub;
        }

        /// <summary>
        /// Gets the plugin manager.
        /// </summary>
        protected IPluginManager PluginManager { get; }

        /// <summary>
        /// Gets the event hub.
        /// </summary>
        protected IEventHub EventHub { get; }

        /// <summary>
        /// Gets a value indicating whether the plugin setup is enabled.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> yielding whether the plugin setp is enabled or not.</returns>
        protected virtual async Task<bool> CanSetupPluginsAsync(IContext context, CancellationToken cancellationToken)
        {
            var setupEvent = new AppSetupQueryEvent { SetupEnabled = true };
            await this.EventHub.PublishAsync(setupEvent, context, cancellationToken).PreserveThreadContext();
            return setupEvent.SetupEnabled;
        }
    }
}
