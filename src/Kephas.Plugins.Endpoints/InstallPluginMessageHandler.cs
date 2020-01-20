// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPluginMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the install plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An install plugin message handler.
    /// </summary>
    public class InstallPluginMessageHandler : MessageHandlerBase<InstallPluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallPluginMessageHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public InstallPluginMessageHandler(IPluginManager pluginManager, IAppContext appContext)
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
        public override async Task<ResponseMessage> ProcessAsync(InstallPluginMessage message, IMessagingContext context, CancellationToken token)
        {
            this.appContext.Logger.Info("Installing plugin {plugin} {version}...", message.Id, message.Version);

            var result = await this.pluginManager.InstallPluginAsync(new PluginIdentity(message.Id, message.Version)).PreserveThreadContext();

            var plugin = result["Plugin"] as IPlugin;
            var pluginId = plugin?.GetTypeInfo().Name ?? message.Id;
            var pluginVersion = plugin?.GetTypeInfo().Version ?? message.Version;

            this.appContext.Logger.Info("Plugin {plugin} {version} installed in {pluginPath}.", pluginId, pluginVersion, plugin?.FolderPath);

            return new ResponseMessage
                {
                    Message = $"Plugin {pluginId} ({pluginVersion}) installed in {plugin?.FolderPath}.",
                };
        }
    }
}
