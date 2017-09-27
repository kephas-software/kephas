// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBrokerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message broker base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base implementation of a <see cref="IMessageBroker"/>.
    /// </summary>
    public abstract class MessageBrokerBase : IMessageBroker
    {
        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        public abstract Task<IMessage> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public BrokeredMessageBuilder CreateBrokeredMessageBuilder()
        {
            return new BrokeredMessageBuilder();
        }
    }
}