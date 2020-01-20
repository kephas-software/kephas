// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAvailablePluginsMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get available plugins message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A get available plugins handler.
    /// </summary>
    public class GetAvailablePluginsMessageHandler : MessageHandlerBase<GetAvailablePluginsMessage, GetAvailablePluginsResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailablePluginsMessageHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public GetAvailablePluginsMessageHandler(IPluginManager pluginManager, IAppContext appContext)
        {
            this.pluginManager = pluginManager;
            this.appContext = appContext;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<GetAvailablePluginsResponseMessage> ProcessAsync(GetAvailablePluginsMessage message, IMessagingContext context, CancellationToken token)
        {
            this.appContext.Logger.Info("Retrieving {count} packages for {search}...", message.Take, message.SearchTerm);

            var plugins = await this.pluginManager.GetAvailablePluginsAsync(f => { f.SearchTerm = message.SearchTerm; f.IncludePrerelease = message.IncludePrerelease; }, cancellationToken: token).PreserveThreadContext();

            this.appContext.Logger.Info("Retrieved {count} packages for {search}...", plugins.Count(), message.SearchTerm);

            return new GetAvailablePluginsResponseMessage
                {
                    Plugins = plugins.Select(p => p.GetIdentity()).ToArray(),
                };
        }
    }
}
