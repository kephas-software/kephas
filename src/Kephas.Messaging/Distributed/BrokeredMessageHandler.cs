// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the brokered message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A brokered message handler.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class BrokeredMessageHandler : MessageHandlerBase<BrokeredMessage, IMessage>
    {
        /// <summary>
        /// The message processor.
        /// </summary>
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageHandler"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        public BrokeredMessageHandler(IMessageProcessor messageProcessor)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));

            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override Task<IMessage> ProcessAsync(BrokeredMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            return this.messageProcessor.ProcessAsync(message.Message, null, token);
        }
    }
}