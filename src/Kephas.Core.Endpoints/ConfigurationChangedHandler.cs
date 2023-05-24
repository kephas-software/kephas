// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationChangedHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Interaction;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Handler for <see cref="ConfigurationChangedSignal"/>.
    /// </summary>
    public class ConfigurationChangedHandler : IMessageHandler<IMessageEnvelope<ConfigurationChangedSignal>, object?>
    {
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;
        private readonly ILogger<ConfigurationChangedHandler>? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The vent hub.</param>
        /// <param name="logger">Optional. The logger.</param>
        public ConfigurationChangedHandler(IAppRuntime appRuntime, IEventHub eventHub, ILogger<ConfigurationChangedHandler>? logger = null)
        {
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
            this.logger = logger;
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
        public async Task<object?> ProcessAsync(IMessageEnvelope<ConfigurationChangedSignal> message, IMessagingContext context, CancellationToken token)
        {
            var signal = message.GetContent();
            if (this.appRuntime.GetAppInstanceId() == signal.SourceAppInstanceId)
            {
                this.logger.Debug($"Ignore {nameof(ConfigurationChangedSignal)} for {{settingsType}}, sent from the same app instance {{app}}.", signal.SettingsType, signal.SourceAppInstanceId);
                return new Response
                {
                    Message = $"Ignore {nameof(ConfigurationChangedSignal)} for {signal.SettingsType}, sent from the same app instance {signal.SourceAppInstanceId}.",
                };
            }

            this.logger.Info($"Received {nameof(ConfigurationChangedSignal)} for {{settingsType}} from app instance {{app}}.", signal.SettingsType, signal.SourceAppInstanceId);

            await this.eventHub.PublishAsync(signal, context, token).PreserveThreadContext();

            return new Response
            {
                Message = $"Received {nameof(ConfigurationChangedSignal)} for {signal.SettingsType} from app instance {signal.SourceAppInstanceId}.",
            };
        }
    }
}