// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the update plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An update plugin message handler.
    /// </summary>
    public class UpdatePluginHandler : MessageHandlerBase<UpdatePluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public UpdatePluginHandler(IPluginManager pluginManager, IAppContext appContext)
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
        public override async Task<ResponseMessage> ProcessAsync(UpdatePluginMessage message, IMessagingContext context, CancellationToken token)
        {
            this.appContext.Logger.Info("Updating plugin {plugin} to version {version}...", message.Id, message.Version);

            var result = await this.pluginManager.UpdatePluginAsync(new AppIdentity(message.Id, message.Version), ctx => ctx.Merge(context), token).PreserveThreadContext();

            var plugin = result.ReturnValue;
            var pluginId = plugin?.GetTypeInfo().Name ?? message.Id;
            var pluginVersion = plugin?.GetTypeInfo().Version ?? message.Version;

            this.appContext.Logger.Info("Plugin {plugin} updated to version {version} in {pluginPath}. Elapsed: {elapsed:c}.", pluginId, pluginVersion, plugin?.Location, result.Elapsed);

            return new ResponseMessage
            {
                Message = $"Plugin {pluginId} ({pluginVersion}) updated in {plugin?.Location}. Elapsed: {result.Elapsed:c}.",
            };
        }
    }
}
