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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
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

            var builder = messageBroker.CreateBrokeredMesssageBuilder(context);
            builder = message is IBrokeredMessage brokeredMessage
                ? builder.Of(brokeredMessage)
                : builder.WithMessageContent(message);

            brokeredMessage = builder
                .WithRecipients(recipients)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }
    }
}