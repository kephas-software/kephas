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
    using System;
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
        /// <param name="messageBroker">The messageBroker to act on.</param>
        /// <param name="event">The event message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result yielding the message ID.
        /// </returns>
        public static async Task<object> PublishAsync(
            this IMessageBroker messageBroker,
            IEvent @event,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(@event, nameof(@event));

            var brokeredMessage = new BrokeredMessage
                                      {
                                          Message = @event,
                                      };

            await messageBroker.DispatchAsync(brokeredMessage, cancellationToken).PreserveThreadContext();

            return brokeredMessage.Id;
        }
    }
}