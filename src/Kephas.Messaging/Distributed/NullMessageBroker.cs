// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullDistributedMessageBroker.cs" company="Quartz Software SRL">
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
    public class NullMessageBroker : IMessageBroker
    {
        /// <summary>
        /// The message processor.
        /// </summary>
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullMessageBroker"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        public NullMessageBroker(IMessageProcessor messageProcessor)
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
        public Task<IMessage> DispatchAsync(IBrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            return this.messageProcessor.ProcessAsync(brokeredMessage, null, cancellationToken);
        }
    }
}