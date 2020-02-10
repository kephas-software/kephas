// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetInstalledPluginsMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get installed plugins message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Plugins;

    /// <summary>
    /// A get installed plugins message handler.
    /// </summary>
    public class GetInstalledPluginsMessageHandler : MessageHandlerBase<GetInstalledPluginsMessage, GetInstalledPluginsResponseMessage>
    {
        private readonly IPluginManager pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetInstalledPluginsMessageHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        public GetInstalledPluginsMessageHandler(IPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
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
        public override async Task<GetInstalledPluginsResponseMessage> ProcessAsync(GetInstalledPluginsMessage message, IMessagingContext context, CancellationToken token)
        {
            var plugins = this.pluginManager.GetInstalledPlugins();

            return new GetInstalledPluginsResponseMessage
            {
                Plugins = plugins.Select(p => this.GetPluginData(p)).ToArray(),
            };
        }

        private PluginData GetPluginData(IPlugin plugin)
        {
            return new PluginData
            {
                Id = plugin.Identity.Id,
                Version = plugin.Identity.Version,
                Location = plugin.Location,
                State = plugin.State,
            };
        }
    }
}
