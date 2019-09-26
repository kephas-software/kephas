// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHubMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event hub message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.AttributedModel;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Events;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An event hub message handler.
    /// </summary>
    [MessageHandler(MessageTypeMatching.TypeOrHierarchy, MessageIdMatching = MessageIdMatching.All)]
    public class EventHubMessageHandler : MessageHandlerBase<IEvent, IMessage>
    {
        /// <summary>
        /// The event hub.
        /// </summary>
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubMessageHandler"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public EventHubMessageHandler(IEventHub eventHub)
        {
            Requires.NotNull(eventHub, nameof(eventHub));

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
        public override async Task<IMessage> ProcessAsync(IEvent message, IMessagingContext context, CancellationToken token)
        {
            await this.eventHub.NotifySubscribersAsync(message, context, token).PreserveThreadContext();

            return null;
        }
    }
}