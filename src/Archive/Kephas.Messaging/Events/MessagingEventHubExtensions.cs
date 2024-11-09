// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingEventHubExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEventHub interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Interaction;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IEventHub"/>.
    /// </summary>
    public static class MessagingEventHubExtensions
    {
        /// <summary>
        /// Subscribes to the event with the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The eventHub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="messageTypeMatching">Optional. The message type matching.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(
            this IEventHub eventHub,
            Func<TEvent, IContext, CancellationToken, Task> callback,
            MessageTypeMatching messageTypeMatching)
            where TEvent : class
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            var messagingEventHub = GetMessagingEventHub(eventHub);
            return messagingEventHub.Subscribe(
                new MessageMatch
                {
                    MessageType = typeof(TEvent),
                    MessageTypeMatching = messageTypeMatching,
                },
                (e, ctx, token) => callback((TEvent)e, ctx, token));
        }

        /// <summary>
        /// Subscribes to the event with the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The eventHub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="messageTypeMatching">Optional. The message type matching.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(
            this IEventHub eventHub,
            Action<TEvent, IContext> callback,
            MessageTypeMatching messageTypeMatching)
            where TEvent : class
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            var messagingEventHub = GetMessagingEventHub(eventHub);
            return messagingEventHub.Subscribe(
                new MessageMatch
                {
                    MessageType = typeof(TEvent),
                    MessageTypeMatching = messageTypeMatching,
                },
                (e, ctx, token) =>
                {
                    callback((TEvent)e, ctx);
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="eventHub">The eventHub to act on.</param>
        /// <param name="match">Specifies the match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe(this IEventHub eventHub, IMessageMatch match, Func<object, IContext, CancellationToken, Task> callback)
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            match = match ?? throw new ArgumentNullException(nameof(match));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            var messagingEventHub = GetMessagingEventHub(eventHub);
            return messagingEventHub.Subscribe(match, callback);
        }


        private static IMessagingEventHub GetMessagingEventHub(IEventHub eventHub)
        {
            if (eventHub is IMessagingEventHub messagingEventHub)
            {
                return messagingEventHub;
            }

            throw new MessagingException(Strings.EventHubExtensions_MissingMessagingEventHubImplementation.FormatWith(eventHub.GetType(), typeof(IMessagingEventHub)));
        }
    }
}