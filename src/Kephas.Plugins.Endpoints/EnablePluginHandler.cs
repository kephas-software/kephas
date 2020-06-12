// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnablePluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enable plugin message handler class.
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
    /// An enable plugin message handler.
    /// </summary>
    public class EnablePluginHandler : MessageHandlerBase<EnablePluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnablePluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        public EnablePluginHandler(
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
        public override async Task<ResponseMessage> ProcessAsync(EnablePluginMessage message, IMessagingContext context, CancellationToken token)
        {
            if (this.appRuntime.PluginsEnabled())
            {
                var signal = new ScheduleStartupCommandSignal(message);
                await this.eventHub.PublishAsync(signal, context, token).PreserveThreadContext();

                return new ResponseMessage
                {
                    Message = $"The {nameof(EnablePluginMessage)} cannot be processed because the plugins are enabled. It has been rescheduled to be executed after the application restart.",
                    Severity = SeverityLevel.Warning,
                };
            }

            this.appContext.Logger.Info("Enabling plugin {plugin}...", message.Id);

            var result = await this.pluginManager.EnablePluginAsync(new AppIdentity(message.Id), ctx => ctx.Merge(context), token).PreserveThreadContext();

            this.appContext.Logger.Info("Plugin {plugin} enabled.", message.Id);

            return new ResponseMessage
            {
                Message = $"Plugin {message.Id} enabled. It will be loaded the next time you restart the system.",
            };
        }
    }
}
