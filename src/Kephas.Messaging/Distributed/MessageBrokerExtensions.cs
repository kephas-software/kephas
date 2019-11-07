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
        /// Publishes an event asynchronously.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event message.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// It does not wait for an answer from the subscribers, just for the acknowledgement of the
        /// message being sent.
        /// </remarks>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));

            return messageBroker.DispatchAsync(@event.ToEvent(), ctx => ctx.Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event message.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// It does not wait for an answer from the subscribers, just for the acknowledgement of the
        /// message being sent.
        /// </remarks>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IEndpoint recipient,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipient, nameof(recipient));

            return messageBroker.DispatchAsync(@event.ToEvent(), ctx => ctx.To(recipient).Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event message.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// It does not wait for an answer from the subscribers, just for the acknowledgement of the
        /// message being sent.
        /// </remarks>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IEnumerable<IEndpoint> recipients,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipients, nameof(recipients));

            return messageBroker.DispatchAsync(@event.ToEvent(), ctx => ctx.To(recipients).Merge(optionsConfig), cancellationToken);
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task PublishAsync<TEvent>(
            this IMessageBroker messageBroker,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TEvent : class, new()
        {
            return messageBroker.PublishAsync(new TEvent(), optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Asynchronously publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventId">Identifier for the event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            string eventId,
            IExpando eventArgs,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return messageBroker.PublishAsync(new IdentifiableEvent { Id = eventId, EventArgs = eventArgs }, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish(
            this IMessageBroker messageBroker,
            object @event,
            Action<IDispatchingContext> optionsConfig = null)
        {
            messageBroker.PublishAsync(@event, optionsConfig).WaitNonLocking();
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish<TEvent>(
            this IMessageBroker messageBroker,
            Action<IDispatchingContext> optionsConfig = null)
            where TEvent : class, new()
        {
            messageBroker.PublishAsync(new TEvent(), optionsConfig).WaitNonLocking();
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event message.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <remarks>
        /// It does not wait for an answer from the subscribers, just for the acknowledgement of the
        /// message being sent.
        /// </remarks>
        public static void Publish(
            this IMessageBroker messageBroker,
            object @event,
            IEndpoint recipient,
            Action<IDispatchingContext> optionsConfig = null)
        {
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipient, nameof(recipient));

            messageBroker.DispatchAsync(@event.ToEvent(), ctx => ctx.To(recipient).Merge(optionsConfig)).WaitNonLocking();
        }

        /// <summary>
        /// Publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventId">Identifier for the application event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish(
            this IMessageBroker messageBroker,
            string eventId,
            IExpando eventArgs,
            Action<IDispatchingContext> optionsConfig = null)
        {
            messageBroker.PublishAsync(new IdentifiableEvent { Id = eventId, EventArgs = eventArgs }, optionsConfig).WaitNonLocking();
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return messageBroker.DispatchAsync(message, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
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
        /// <param name="messageBroker">The message broker.</param>
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
        /// <param name="messageBroker">The message broker.</param>
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
        /// <param name="messageBroker">The message broker.</param>
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
        /// <param name="messageBroker">The message broker.</param>
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