// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBrokerExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message broker extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging.Events;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IMessageBroker"/>.
    /// </summary>
    public static class MessageBrokerExtensions
    {
        /// <summary>
        /// Dispatches the message asynchronously.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the dispatch.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IMessage> DispatchAsync(
            this IMessageBroker messageBroker,
            Action<IDispatchingContext> optionsConfig,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));

            return messageBroker.DispatchAsync(null, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <remarks>
        /// It does not wait for an answer from the subscribers, just for the acknowledgement of the
        /// message being sent.
        /// </remarks>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="event">The event message.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));

            return messageBroker.DispatchAsync(ctx => ctx.ContentEvent(@event), cancellationToken);
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <remarks>
        /// It does not wait for an answer from the subscribers,
        /// just for the acknowledgement of the message being sent.
        /// </remarks>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="event">The event message.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IEndpoint recipient,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipient, nameof(recipient));

            return messageBroker.DispatchAsync(ctx => ctx.ContentEvent(@event).To(recipient), cancellationToken);
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <remarks>
        /// It does not wait for an answer from the subscribers,
        /// just for the acknowledgement of the message being sent.
        /// </remarks>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="event">The event message.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IEnumerable<IEndpoint> recipients,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipients, nameof(recipients));

            return messageBroker.DispatchAsync(ctx => ctx.ContentEvent(@event).To(recipients), cancellationToken);
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task PublishAsync<TEvent>(
            this IMessageBroker messageBroker,
            CancellationToken cancellationToken = default)
            where TEvent : class, new()
        {
            return messageBroker.PublishAsync(new TEvent(), cancellationToken);
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish(this IMessageBroker messageBroker, object @event)
        {
            messageBroker.PublishAsync(@event).WaitNonLocking();
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageBroker">The message broker.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish<TEvent>(this IMessageBroker messageBroker)
            where TEvent : class, new()
        {
            messageBroker.PublishAsync(new TEvent()).WaitNonLocking();
        }

        /// <summary>
        /// Asynchronously publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventId">Identifier for the event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object eventId,
            IExpando eventArgs,
            CancellationToken cancellationToken = default)
        {
            return messageBroker.PublishAsync(new IdentifiableEvent { Id = eventId, EventArgs = eventArgs }, cancellationToken);
        }

        /// <summary>
        /// Publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventId">Identifier for the application event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish(
            this IMessageBroker messageBroker,
            object eventId,
            IExpando eventArgs)
        {
            messageBroker.PublishAsync(new IdentifiableEvent { Id = eventId, EventArgs = eventArgs }).WaitNonLocking();
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IEndpoint recipient,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(recipient, nameof(recipient));

            return messageBroker.DispatchAsync(message, ctx => ctx.To(recipient).Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IEnumerable<IEndpoint> recipients,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(recipients, nameof(recipients));

            return messageBroker.DispatchAsync(message, ctx => ctx.To(recipients).Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return messageBroker.DispatchAsync(message, ctx => ctx.OneWay().Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            IEndpoint recipient,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(recipient, nameof(recipient));

            return messageBroker.DispatchAsync(message, ctx => ctx.To(recipient).OneWay().Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            IEnumerable<IEndpoint> recipients,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(recipients, nameof(recipients));

            return messageBroker.DispatchAsync(message, ctx => ctx.To(recipients).OneWay().Merge(optionsConfig), cancellationToken);
        }
    }
}