// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHub.cs" company="Kephas Software SRL">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default event hub.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultEventHub : Loggable, IEventHub, IAsyncFinalizable
    {
        private readonly IMessageMatchService messageMatchService;
        private readonly IList<EventSubscription> subscriptions = new List<EventSubscription>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEventHub"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerRegistry">The handler registry.</param>
        public DefaultEventHub(IMessageMatchService messageMatchService, IMessageHandlerRegistry handlerRegistry)
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

            var subscription = new EventSubscription(
                match,
                callback,
                s =>
                {
                    lock (this.subscriptions)
                    {
                        this.subscriptions.Remove(s);
                    }
                });
            lock (this.subscriptions)
            {
                this.subscriptions.Add(subscription);
            }

            return subscription;
        }

        /// <summary>
        /// Publishes the event asynchronously to its subscribers.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual async Task PublishAsync(object @event, IContext context, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));

            IList<EventSubscription> subscriptionsCopy;
            lock (this.subscriptions)
            {
                var eventType = this.messageMatchService.GetMessageType(@event);
                var eventId = this.messageMatchService.GetMessageId(@event);
                subscriptionsCopy = this.subscriptions.Where(s => this.messageMatchService.IsMatch(s.Match, @event.GetType(), eventType, eventId)).ToList();
            }

            foreach (var subscription in subscriptionsCopy)
            {
                try
                {
                    var task = subscription.Callback?.Invoke(@event.GetContent(), context, cancellationToken);
                    if (task != null)
                    {
                        await task.PreserveThreadContext();
                    }
                }
                catch (Exception ex)
                {
                    // TODO localization
                    this.Logger.Error(ex, $"An error occurred when invoking subscription for {@event.GetType()}.");
                }
            }
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            lock (this.subscriptions)
            {
                this.subscriptions.Clear();
            }

            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// An event subscription.
        /// </summary>
        private class EventSubscription : IEventSubscription
        {
            private readonly Action<EventSubscription> onDispose;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventSubscription"/> class.
            /// </summary>
            /// <param name="match">Specifies the match.</param>
            /// <param name="callback">The callback.</param>
            /// <param name="onDispose">The on dispose.</param>
            public EventSubscription(IMessageMatch match, Func<object, IContext, CancellationToken, Task> callback, Action<EventSubscription> onDispose)
            {
                this.Match = match;
                this.Callback = callback;
                this.onDispose = onDispose;
            }

            /// <summary>
            /// Gets the match.
            /// </summary>
            /// <value>
            /// The match.
            /// </value>
            public IMessageMatch Match { get; }

            /// <summary>
            /// Gets the callback.
            /// </summary>
            /// <value>
            /// The callback.
            /// </value>
            public Func<object, IContext, CancellationToken, Task> Callback { get; private set; }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
            /// resources.
            /// </summary>
            public void Dispose()
            {
                if (this.Callback == null)
                {
                    return;
                }

                this.Callback = null;
                this.onDispose(this);
            }

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return $"{this.GetType().Name}:{this.Match}";
            }
        }
    }
}