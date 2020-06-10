// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingEventHub.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Interaction;
    using Kephas.Messaging.Composition;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A messaging event hub.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class MessagingEventHub : DefaultEventHub, IMessagingEventHub
    {
        private readonly IMessageMatchService messageMatchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEventHub"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerRegistry">The handler registry.</param>
        public MessagingEventHub(IMessageMatchService messageMatchService, IMessageHandlerRegistry handlerRegistry)
        {
            Requires.NotNull(messageMatchService, nameof(messageMatchService));
            Requires.NotNull(handlerRegistry, nameof(handlerRegistry));

            this.messageMatchService = messageMatchService;
            handlerRegistry.RegisterHandler(
                new FuncMessageHandler<object>(async (e, ctx, token) =>
                {
                    await this.PublishAsync(e, ctx, token).PreserveThreadContext();
                    return null;
                }),
                new MessageHandlerMetadata(envelopeType: typeof(IEvent), envelopeTypeMatching: MessageTypeMatching.TypeOrHierarchy, messageIdMatching: MessageIdMatching.All));
        }

        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="match">Specifies the match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public virtual IEventSubscription Subscribe(IMessageMatch match, Func<object, IContext, CancellationToken, Task> callback)
        {
            Requires.NotNull(match, nameof(match));
            Requires.NotNull(callback, nameof(callback));

            Func<object, bool> funcMatch = e => this.messageMatchService.IsMatch(match, e.GetType(), this.messageMatchService.GetMessageType(e), this.messageMatchService.GetMessageId(e));
            return this.Subscribe(funcMatch, callback);
        }

        /// <summary>
        /// Gets the event content.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>
        /// The event content.
        /// </returns>
        protected override object GetEventContent(object @event)
        {
            return @event.GetContent();
        }

        /// <summary>
        /// Gets the match for the provided event type.
        /// </summary>
        /// <param name="typeMatch">Specifies the type match criteria.</param>
        /// <returns>
        /// A function delegate that yields a bool.
        /// </returns>
        protected override Func<object, bool> GetTypeMatch(Type typeMatch)
        {
            var match = new MessageMatch
            {
                MessageType = typeMatch,
                MessageTypeMatching = MessageTypeMatching.Type,
            };
            return e => this.messageMatchService.IsMatch(match, e.GetType(), this.messageMatchService.GetMessageType(e), this.messageMatchService.GetMessageId(e));
        }
    }
}