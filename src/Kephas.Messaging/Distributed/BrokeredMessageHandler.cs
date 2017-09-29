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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.ExceptionHandling;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

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
        /// The message broker factory.
        /// </summary>
        private readonly IExportFactory<IMessageBroker> messageBrokerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageHandler"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="messageBrokerFactory">The message broker factory.</param>
        public BrokeredMessageHandler(
            IMessageProcessor messageProcessor,
            IExportFactory<IMessageBroker> messageBrokerFactory)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));
            Requires.NotNull(messageBrokerFactory, nameof(messageBrokerFactory));

            this.messageProcessor = messageProcessor;
            this.messageBrokerFactory = messageBrokerFactory;
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
        public override async Task<IMessage> ProcessAsync(BrokeredMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            if (message.ReplyToMessageId != null)
            {
                // do not wait for the processing.
                Task.Factory.StartNew(() => this.ProcessReplyAsync(message, context, token));
            }
            else if (message.IsOneWay)
            {
                // do not wait for the processing.
                Task.Factory.StartNew(() => this.ProcessOneWayAsync(message, context, token));
            }
            else
            {
                // wait for the processing and return the result through the message broker.
                Task.Factory.StartNew(() => this.ProcessAndRespondAsync(message, context, token));
            }

            return null;
        }

        /// <summary>
        /// Process the one way message asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        private Task ProcessOneWayAsync(
            BrokeredMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            return this.messageProcessor.ProcessAsync(message.Content, context, token);
        }

        /// <summary>
        /// Process the reply asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        private async Task ProcessReplyAsync(
            BrokeredMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            var broker = this.messageBrokerFactory.CreateExportedValue();

            await broker.ReplyReceivedAsync(message, token);
        }

        /// <summary>
        /// Process the message and respond asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        private async Task ProcessAndRespondAsync(
            BrokeredMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            IMessage response = null;
            Exception exception = null;
            try
            {
                response = await this.messageProcessor.ProcessAsync(message.Content, context, token).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                response = new ExceptionMessage { Exception = new ExceptionData(exception) };
            }

            var broker = this.messageBrokerFactory.CreateExportedValue();
            var responseMessage = broker.CreateBrokeredMessageBuilder()
                .WithContent(response)
                .OneWay()
                .ReplyTo(message.Id, message.Sender)
                .BrokeredMessage;

            await broker.DispatchAsync(responseMessage, token).PreserveThreadContext();
        }
    }
}