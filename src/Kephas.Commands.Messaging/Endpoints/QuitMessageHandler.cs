// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuitMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quit message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A quit message handler.
    /// </summary>
    public class QuitMessageHandler : IActionMessageHandler<QuitMessage>
    {
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuitMessageHandler"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public QuitMessageHandler(IEventHub eventHub)
        {
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
        public async Task ProcessAsync(QuitMessage message, IMessagingContext context, CancellationToken token)
        {
            await this.eventHub.PublishAsync<InteractionContext>(
                new ShutdownSignal("Shutdown triggered by user"),
                ctx => ctx.Impersonate(context).OneWay().WithDelay(TimeSpan.FromSeconds(2)),
                token).PreserveThreadContext();
        }
    }
}
