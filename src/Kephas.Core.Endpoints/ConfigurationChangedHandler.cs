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
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Handler for <see cref="ConfigurationChangedSignal"/>.
    /// </summary>
    public class ConfigurationChangedHandler : MessageHandlerBase<ConfigurationChangedSignal, ResponseMessage>
    {
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The vent hub.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public ConfigurationChangedHandler(IAppRuntime appRuntime, IEventHub eventHub, ILogManager? logManager = null)
            : base(logManager)
        {
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
        public override async Task<ResponseMessage> ProcessAsync(ConfigurationChangedSignal message, IMessagingContext context, CancellationToken token)
        {
            if (this.appRuntime.GetAppInstanceId() == message.SourceAppInstanceId)
            {
                this.Logger.Info($"Ignore {nameof(ConfigurationChangedSignal)}, sent from the same app instance {{app}}.", message.SourceAppInstanceId);
                return new ResponseMessage
                {
                    Message = $"Ignore {nameof(ConfigurationChangedSignal)}, sent from the same app instance {message.SourceAppInstanceId}.",
                };
            }

            this.Logger.Info($"Received {nameof(ConfigurationChangedSignal)} from app instance {{app}}.", message.SourceAppInstanceId);

            await this.eventHub.PublishAsync(message, context, token).PreserveThreadContext();

            return new ResponseMessage
            {
                Message = $"Notified {nameof(ConfigurationChangedSignal)} to app instance {message.SourceAppInstanceId}.",
            };
        }
    }
}