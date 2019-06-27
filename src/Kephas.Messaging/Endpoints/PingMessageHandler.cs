// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Message handler for the <see cref="PingMessage" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Messages;

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
            return Task.FromResult(new PingBackMessage
                                       {
                                           ServerTime = DateTimeOffset.Now,
                                       });
        }
    }
}