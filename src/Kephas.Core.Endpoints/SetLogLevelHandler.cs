﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetLogLevelHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the set log level handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A set log level handler.
    /// </summary>
    public class SetLogLevelHandler : IMessageHandler<SetLogLevelMessage, Response>
    {
        private readonly ILogManager logManager;
        private readonly ILogger<SetLogLevelHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetLogLevelHandler"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <param name="logger">The logger.</param>
        public SetLogLevelHandler(ILogManager logManager, ILogger<SetLogLevelHandler> logger)
        {
            this.logManager = logManager;
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
        public async Task<Response> ProcessAsync(SetLogLevelMessage message, IMessagingContext context, CancellationToken token)
        {
            this.logger.Warn("Changing the minimum log level from {oldLogLevel} to {logLevel} by '{user}'.", this.logManager.MinimumLevel, message.MinimumLevel, context.Identity?.Name);
            this.logManager.MinimumLevel = message.MinimumLevel;
            this.logger.Warn("The minimum log level changed to {logLevel} by '{user}'.", this.logManager.MinimumLevel, context.Identity?.Name);

            return new Response
            {
                Message = $"The application's minimum log level was set to {this.logManager.MinimumLevel}.",
            };
        }
    }
}