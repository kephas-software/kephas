// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBroker.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null distributed message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A distributed message broker sending the messages to the message processor.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class InProcessMessageBroker : IMessageBroker
    {
        /// <summary>
        /// The message processor.
        /// </summary>
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessMessageBroker"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        public InProcessMessageBroker(IMessageProcessor messageProcessor)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));

            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        public virtual Task<IMessage> DispatchAsync(IBrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));

            return this.messageProcessor.ProcessAsync(brokeredMessage, null, cancellationToken);
        }

        /// <summary>
        /// Publishes an event asynchronously.
        /// </summary>
        /// <remarks>
        /// It does not wait for an answer from the subscribers,
        /// just for the aknowledgement of the message being sent.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task PublishAsync(IBrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            return this.DispatchAsync(brokeredMessage, cancellationToken);
        }

        /// <summary>
        /// Processes a message asynchronously, waiting for a response from the handler.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result yielding the response message.
        /// </returns>
        public virtual Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            return this.DispatchAsync(brokeredMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public virtual BrokeredMessageBuilder CreateBrokeredMessageBuilder()
        {
            return new BrokeredMessageBuilder();
        }
    }
}