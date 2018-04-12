// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBrokerExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message broker extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
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
        /// just for the aknowledgement of the message being sent.
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
            IEvent @event,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));

            if (!(@event is IBrokeredMessage brokeredMessage))
            {
                var brokeredMessageBuilder = messageBroker.CreateBrokeredMessageBuilder(context);
                brokeredMessage = brokeredMessageBuilder.WithContent(@event).OneWay().BrokeredMessage;
            }
            else
            {
                // TODO check the message broker content
            }

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
            IMessage message,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));

            if (!(message is IBrokeredMessage brokeredMessage))
            {
                var brokeredMessageBuilder = messageBroker.CreateBrokeredMessageBuilder(context);
                brokeredMessage = brokeredMessageBuilder
                                    .WithContent(message)
                                    .BrokeredMessage;
            }
            else
            {
                // TODO check the message broker content
                // brokeredMessage.IsOneWay = false
            }

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
            IMessage message,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));

            if (!(message is IBrokeredMessage brokeredMessage))
            {
                var brokeredMessageBuilder = messageBroker.CreateBrokeredMessageBuilder(context);
                brokeredMessage = brokeredMessageBuilder
                    .WithContent(message)
                    .OneWay()
                    .BrokeredMessage;
            }
            else
            {
                // TODO check the message broker content
                // brokeredMessage.IsOneWay = true
            }

            return messageBroker.DispatchAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Creates a untyped brokered message builder.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="context">The sending context (optional).</param>
        /// <returns>
        /// The new untyped brokered message builder.
        /// </returns>
        public static BrokeredMessageBuilder<BrokeredMessage> CreateBrokeredMessageBuilder(this IMessageBroker messageBroker, IContext context = null)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));

            return messageBroker.CreateBrokeredMessageBuilder<BrokeredMessage>(context);
        }
    }
}