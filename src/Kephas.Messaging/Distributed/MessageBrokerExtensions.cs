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
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="IMessageBroker"/>.
    /// </summary>
    public static class MessageBrokerExtensions
    {
        private static readonly MethodInfo CreateMessageBuilderMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((IMessageBroker)null).CreateBrokeredMessageBuilder<IBrokeredMessage>((IContext)null, (IBrokeredMessage)null));

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
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            object @event,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));

            var brokeredEvent = @event as IBrokeredMessage;
            var brokeredMessageBuilder = brokeredEvent == null
                                             ? messageBroker.CreateBrokeredMessageBuilder(context)
                                             : messageBroker.CreateBrokeredMessageBuilder(brokeredEvent.GetType(), context, brokeredEvent);
            var brokeredMessage = brokeredMessageBuilder
                .WithEventContent(@event)
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
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));

            var brokeredMessage = message as IBrokeredMessage;
            var brokeredMessageBuilder = brokeredMessage == null
                                             ? messageBroker.CreateBrokeredMessageBuilder(context)
                                             : messageBroker.CreateBrokeredMessageBuilder(brokeredMessage.GetType(), context, brokeredMessage);
            brokeredMessage = brokeredMessageBuilder
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
        /// <param name="context">Optional. The processing context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            object message,
            IEndpoint recipient,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(recipient, nameof(recipient));

            var brokeredMessage = message as IBrokeredMessage;
            var brokeredMessageBuilder = brokeredMessage == null
                                             ? messageBroker.CreateBrokeredMessageBuilder(context)
                                             : messageBroker.CreateBrokeredMessageBuilder(brokeredMessage.GetType(), context, brokeredMessage);
            brokeredMessage = brokeredMessageBuilder
                .WithMessageContent(message)
                .WithRecipients(recipient)
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            object message,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));

            var brokeredMessage = message as IBrokeredMessage;
            var brokeredMessageBuilder = brokeredMessage == null
                                             ? messageBroker.CreateBrokeredMessageBuilder(context)
                                             : messageBroker.CreateBrokeredMessageBuilder(brokeredMessage.GetType(), context, brokeredMessage);
            brokeredMessage = brokeredMessageBuilder
                .WithMessageContent(message)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="messageType">Type of the message. It is used to identify the brokered message builder.</param>
        /// <param name="context">Optional. The sending context.</param>
        /// <param name="brokeredMessage">Optional. The brokered message.</param>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public static IBrokeredMessageBuilder CreateBrokeredMessageBuilder(
            this IMessageBroker messageBroker,
            Type messageType,
            IContext context = null,
            IBrokeredMessage brokeredMessage = null)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(messageType, nameof(messageType));

            var createBuilder = CreateMessageBuilderMethod.MakeGenericMethod(messageType);
            var builder = (IBrokeredMessageBuilder)createBuilder.Call(messageBroker, context, brokeredMessage);
            return builder;
        }

        /// <summary>
        /// Creates a untyped brokered message builder.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="context">Optional. The sending context.</param>
        /// <param name="brokeredMessage">Optional. The brokered message.</param>
        /// <returns>
        /// The new untyped brokered message builder.
        /// </returns>
        public static IBrokeredMessageBuilder CreateBrokeredMessageBuilder(
            this IMessageBroker messageBroker,
            IContext context = null,
            BrokeredMessage brokeredMessage = null)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));

            return messageBroker.CreateBrokeredMessageBuilder<BrokeredMessage>(context, brokeredMessage);
        }
    }
}