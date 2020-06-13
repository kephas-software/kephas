// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the install plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Interaction;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An install plugin message handler.
    /// </summary>
    public class InstallPluginHandler : PluginHandlerBase<InstallPluginMessage, ResponseMessage>
    {
        private readonly IAppContext appContext;
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallPluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public InstallPluginHandler(
            IPluginManager pluginManager,
            IAppContext appContext,
            IAppRuntime appRuntime,
            IEventHub eventHub,
            ILogManager? logManager = null)
            : base(pluginManager, eventHub, logManager)
        {
            this.appContext = appContext;
            this.appRuntime = appRuntime;
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
            if (!await this.CanSetupPluginsAsync(context, token).PreserveThreadContext())
            {
                var signal = new ScheduleStartupCommandSignal(message);
                await this.EventHub.PublishAsync(signal, context, token).PreserveThreadContext();

                return new ResponseMessage
                {
                    Message = $"The {nameof(InstallPluginMessage)} cannot be processed because the plugins are enabled. It has been rescheduled to be executed after the application restart.",
                    Severity = SeverityLevel.Warning,
                };
            }

            this.appContext.Logger.Info("Installing plugin {plugin} {version}...", message.Id, message.Version);

            AppIdentity? pluginIdentity;
            if (UpdatePluginMessage.LatestVersion.Equals(message.Version, StringComparison.InvariantCultureIgnoreCase))
            {
                var availablePackage = await this.PluginManager.GetLatestAvailablePluginVersionAsync(
                    new AppIdentity(message.Id),
                    message.IncludePrerelease,
                    token).PreserveThreadContext();
                pluginIdentity = availablePackage?.Identity;
            }
            else
            {
                pluginIdentity = new AppIdentity(message.Id, message.Version);
            }

            if (pluginIdentity == null)
            {
                this.appContext.Logger.Info(
                    "No plugin {plugin} {version} could be found.",
                    message.Id,
                    message.Version);
                return new ResponseMessage
                {
                    Message = $"No plugin {message.Id} {message.Version} could be found.",
                };
            }

            var result = await this.PluginManager.InstallPluginAsync(pluginIdentity, ctx => ctx.Merge(context), token)
                .PreserveThreadContext();

            var plugin = result.Value;
            var pluginId = plugin?.GetTypeInfo().Name ?? message.Id;
            var pluginVersion = plugin?.GetTypeInfo().Identity.Version?.ToString() ?? message.Version;

            this.appContext.Logger.Info(
                "Plugin {plugin} {version} ({state}) installed in {pluginPath}. Elapsed: {elapsed:c}.",
                pluginId,
                pluginVersion,
                plugin?.State,
                plugin?.Location,
                result.Elapsed);

            return new ResponseMessage
                {
                    Message = $"Plugin {pluginId} {pluginVersion} ({plugin?.State}) installed in {plugin?.Location}. Elapsed: {result.Elapsed:c}.",
                };
        }
    }
}
