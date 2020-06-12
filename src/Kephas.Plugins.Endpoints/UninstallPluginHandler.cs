// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UninstallPluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the uninstall plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
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
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An uninstall plugin message handler.
    /// </summary>
    public class UninstallPluginHandler : MessageHandlerBase<UninstallPluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="UninstallPluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        public UninstallPluginHandler(
            IPluginManager pluginManager,
            IAppContext appContext,
            IAppRuntime appRuntime,
            IEventHub eventHub)
        {
            this.pluginManager = pluginManager;
            this.appContext = appContext;
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
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
        public override async Task<ResponseMessage> ProcessAsync(UninstallPluginMessage message, IMessagingContext context, CancellationToken token)
        {
            if (this.appRuntime.PluginsEnabled())
            {
                var signal = new ScheduleStartupCommandSignal(message);
                await this.eventHub.PublishAsync(signal, context, token).PreserveThreadContext();

                return new ResponseMessage
                {
                    Message = $"The {nameof(UninstallPluginMessage)} cannot be processed because the plugins are enabled. It has been rescheduled to be executed after the application restart.",
                    Severity = SeverityLevel.Warning,
                };
            }

            this.appContext.Logger.Info("Uninstalling plugin {plugin}...", message.Id);

            var result = await this.pluginManager.UninstallPluginAsync(new AppIdentity(message.Id), ctx => ctx.Merge(context), token).PreserveThreadContext();

            var operation = result.Value?.State == PluginState.PendingUninstallation
                ? "uninitialized"
                : "uninstalled";

            this.appContext.Logger.Info("Plugin {plugin} {operation} ({state}). Elapsed: {elapsed:c}.", result.Value?.Id ?? message.Id, operation, result.Value?.State, result.Elapsed);

            return new ResponseMessage
            {
                Message = $"Plugin {message.Id} {operation}. Elapsed: {result.Elapsed:c}.",
            };
        }
    }
}
