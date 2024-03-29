﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingEventHub.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Interaction;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Threading.Tasks;

namespace Kephas.Messaging.Events;

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
    /// <param name="injectableFactory">The injectable factory.</param>
    /// <param name="messageMatchService">The message match service.</param>
    /// <param name="handlerRegistry">The handler registry.</param>
    /// <param name="logManager">Optional. The log manager.</param>
    public MessagingEventHub(
        IInjectableFactory injectableFactory,
        IMessageMatchService messageMatchService,
        IMessageHandlerRegistry handlerRegistry,
        ILogManager? logManager = null)
        : base(injectableFactory, logManager)
    {
        this.messageMatchService = messageMatchService ?? throw new ArgumentNullException(nameof(messageMatchService));
        handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));

        handlerRegistry.RegisterHandler(
            new FuncMessageHandler<IEvent, object?>(async (e, ctx, token) =>
            {
                await this.PublishAsync(e, ctx, token).PreserveThreadContext();
                return null;
            }),
            new MessageHandlerMetadata(envelopeType: typeof(IEvent), envelopeTypeMatching: MessageTypeMatching.TypeOrHierarchy));
    }

    /// <summary>
    /// Subscribes to the event(s) matching the criteria.
    /// </summary>
    /// <param name="match">Specifies the match criteria.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>
    /// An IEventSubscription.
    /// </returns>
    public virtual IEventSubscription Subscribe(IMessageMatch match, Func<object, IContext?, CancellationToken, Task> callback)
    {
        match = match ?? throw new ArgumentNullException(nameof(match));
        callback = callback ?? throw new ArgumentNullException(nameof(callback));

        bool FuncMatch(object e) => this.messageMatchService.IsMatch(match, (IMessagingContext)e.GetType());
        return this.Subscribe(FuncMatch, callback);
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
        return e => this.messageMatchService.IsMatch((IMessageMatch)match, (IMessagingContext)e.GetType());
    }
}