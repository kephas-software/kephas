// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingMessageHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Message handler for the <see cref="PingMessage" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Ping
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Message handler for the <see cref="PingMessage"/>.
    /// </summary>
    public class PingMessageHandler : MessageHandlerBase<PingMessage, PingBackMessage>
    {
        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override Task<PingBackMessage> ProcessAsync(PingMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            return Task.FromResult(new PingBackMessage { ServerTime = DateTimeOffset.Now });
        }
    }
}