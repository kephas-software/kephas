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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging.Events;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IMessageBroker"/>.
    /// </summary>
    public static class MessageBrokerExtensions
    {
        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <remarks>
        /// It does not wait for an answer from the subscribers,
        /// just for the acknowledgement of the message being sent.
        /// </remarks>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="event">The event message.</param>
        /// <param name="context">The publishing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            var brokeredMessageBuilder = @event is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithEventContent(@event);

            brokeredMessage = brokeredMessageBuilder
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
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
        /// <param name="context">The publishing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IEndpoint recipient,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipient, nameof(recipient));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            var brokeredMessageBuilder = @event is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithEventContent(@event);

            brokeredMessage = brokeredMessageBuilder
                .WithRecipients(recipient)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
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
        /// <param name="context">The publishing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IEnumerable<IEndpoint> recipients,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));
            Requires.NotNull(recipients, nameof(recipients));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            var brokeredMessageBuilder = @event is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithEventContent(@event);

            brokeredMessage = brokeredMessageBuilder
                .WithRecipients(recipients)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task PublishAsync<TEvent>(
            this IMessageBroker messageBroker,
            IContext context,
            CancellationToken cancellationToken = default)
            where TEvent : class, new()
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(context, nameof(context));

            return messageBroker.PublishAsync(new TEvent(), context, cancellationToken);
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        public static void Publish(this IMessageBroker messageBroker, object @event, IContext context)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(context, nameof(context));

            messageBroker.PublishAsync(@event, context).WaitNonLocking();
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="context">The context.</param>
        public static void Publish<TEvent>(this IMessageBroker messageBroker, IContext context)
            where TEvent : class, new()
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(context, nameof(context));

            var @event = new TEvent();
            messageBroker.PublishAsync(@event, context).WaitNonLocking();
        }

        /// <summary>
        /// Asynchronously publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventId">Identifier for the event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object eventId,
            IExpando eventArgs,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(context, nameof(context));

            var appEvent = new IdentifiableEvent { Id = eventId, EventArgs = eventArgs };
            return messageBroker.PublishAsync(appEvent, context, cancellationToken);
        }

        /// <summary>
        /// Publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventId">Identifier for the application event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Publish(
            this IMessageBroker messageBroker,
            object eventId,
            IExpando eventArgs,
            IContext context)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(context, nameof(context));

            var @event = new IdentifiableEvent { Id = eventId, EventArgs = eventArgs };
            messageBroker.PublishAsync(@event, context).WaitNonLocking();
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(context, nameof(context));

            if (message is IBrokeredMessage brokeredMessage)
            {
                return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
            }

            var builder = context.CreateBrokeredMessageBuilder();
            brokeredMessage = builder
                .WithMessageContent(message)
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IEndpoint recipient,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(recipient, nameof(recipient));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            var brokeredMessageBuilder = message is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithMessageContent(message);

            brokeredMessage = brokeredMessageBuilder
                .WithRecipients(recipient)
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IEnumerable<IEndpoint> recipients,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(recipients, nameof(recipients));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            var brokeredMessageBuilder = message is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithMessageContent(message);

            brokeredMessage = brokeredMessageBuilder
                .WithRecipients(recipients)
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            builder = message is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithMessageContent(message);

            brokeredMessage = builder
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            IEndpoint recipient,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(recipient, nameof(recipient));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            builder = message is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithMessageContent(message);

            brokeredMessage = builder
                .WithRecipients(recipient)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="recipients">The recipients.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            IEnumerable<IEndpoint> recipients,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(recipients, nameof(recipients));
            Requires.NotNull(context, nameof(context));

            var builder = context.CreateBrokeredMessageBuilder();
            builder = message is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithMessageContent(message);

            brokeredMessage = builder
                .WithRecipients(recipients)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Creates an initialized brokered message builder.
        /// </summary>
        /// <param name="context">The publishing context.</param>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public static IBrokeredMessageBuilder CreateBrokeredMessageBuilder(this IContext context)
        {
            Requires.NotNull(context, nameof(context));

            var builderFactory = context.CompositionContext.GetExportFactory<IBrokeredMessageBuilder>();
            return builderFactory.CreateInitializedValue(context);
        }
    }
}