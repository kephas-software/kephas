// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisablePluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the disable plugin message handler class.
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
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A disable plugin message handler.
    /// </summary>
    public class DisablePluginHandler : PluginHandlerBase<DisablePluginMessage, Response>
    {
        private readonly IAppContext appContext;
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisablePluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logger">The log manager.</param>
        public DisablePluginHandler(
            IPluginManager pluginManager,
            IAppContext appContext,
            IAppRuntime appRuntime,
            IEventHub eventHub,
            ILogger<DisablePluginHandler>? logger = null)
            : base(pluginManager, eventHub, logger)
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
        public override async Task<Response> ProcessAsync(DisablePluginMessage message, IMessagingContext context, CancellationToken token)
        {
            if (!await this.CanSetupPluginsAsync(context, token).PreserveThreadContext())
            {
                var signal = new ScheduleStartupCommandSignal(message);
                await this.EventHub.PublishAsync(signal, context, token).PreserveThreadContext();

                return new Response
                {
                    Message = $"The {nameof(DisablePluginMessage)} cannot be processed because the plugins are enabled. It has been rescheduled to be executed after the application restart.",
                    Severity = SeverityLevel.Warning,
                };
            }

            this.appContext.Logger.Info("Disabling plugin {plugin}...", message.Id);

            var result = await this.PluginManager.DisablePluginAsync(new AppIdentity(message.Id), ctx => ctx.Merge(context), token).PreserveThreadContext();

            this.appContext.Logger.Info("Plugin {plugin} disabled.", message.Id);

            return new Response
            {
                Message = $"Plugin {message.Id} disabled. It will be skipped the next time you restart the system.",
            };
        }
    }
}
