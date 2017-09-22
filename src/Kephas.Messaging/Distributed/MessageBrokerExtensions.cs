// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBrokerExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// just for the aknowledgement of the message being sent.
        /// </remarks>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="event">The event message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IMessageBroker messageBroker,
            IEvent @event,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));

            var brokeredMessage = new BrokeredMessageBuilder()
                .WithMessage(@event)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public static async Task<IMessage> ProcessAsync(
            this IMessageBroker messageBroker,
            IMessage message,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));

            var brokeredMessage = new BrokeredMessageBuilder()
                .WithMessage(message)
                .BrokeredMessage;

            var response = await messageBroker.DispatchAsync(brokeredMessage, cancellationToken).PreserveThreadContext();

            return response;
        }

        /// <summary>
        /// Processes a message asynchronously without waiting for a response from the handler.
        /// </summary>
        /// <param name="messageBroker">The message broker to act on.</param>
        /// <param name="message">The message to be processed.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public static Task<IMessage> ProcessOneWayAsync(
            this IMessageBroker messageBroker,
            IMessage message,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(message, nameof(message));

            var brokeredMessage = new BrokeredMessageBuilder()
                .WithMessage(message)
                .OneWay()
                .BrokeredMessage;

            return messageBroker.DispatchAsync(brokeredMessage, cancellationToken);
        }
    }
}