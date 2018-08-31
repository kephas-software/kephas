// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventHub.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEventHub interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Composition;
    using Kephas.Services;

    /// <summary>
    /// Interface for event hub.
    /// </summary>
    [SharedAppServiceContract]
    public interface IEventHub
    {
        /// <summary>
        /// Publishes asynchronously the event to its subscribers.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task PublishAsync(IEvent @event, IContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="match">Specifies the match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        IEventSubscription Subscribe(IMessageMatch match, Func<IEvent, IContext, CancellationToken, Task> callback);
    }

    /// <summary>
    /// Extension methods for <see cref="IEventHub"/>.
    /// </summary>
    public static class EventHubExtensions
    {
        /// <summary>
        /// Subscribes to the event with the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The eventHub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="messageTypeMatching">The message type matching (optional).</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(
            this IEventHub eventHub,
            Func<TEvent, IContext, CancellationToken, Task> callback,
            MessageTypeMatching messageTypeMatching = MessageTypeMatching.Type)
            where TEvent : IEvent
        {
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(callback, nameof(callback));

            return eventHub.Subscribe(
                new MessageMatch
                    {
                        MessageType = typeof(TEvent),
                        MessageTypeMatching = messageTypeMatching
                    },
                (e, ctx, token) => callback((TEvent)e, ctx, token));
        }
    }
}