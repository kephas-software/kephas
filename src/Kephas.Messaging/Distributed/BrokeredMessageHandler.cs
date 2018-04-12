// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Logging;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
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
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<BrokeredMessageHandler> Logger { get; set; }

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
                Task.Factory.StartNew(() => this.ProcessReply(message, context, token), token);
            }
            else if (message.IsOneWay)
            {
                // do not wait for the processing.
                Task.Factory.StartNew(() => this.ProcessOneWay(message, context, token), token);
            }
            else
            {
                // wait for the processing and return the result through the message broker.
                Task.Factory.StartNew(() => this.ProcessAndRespond(message, context, token), token);
            }

            return null;
        }

        /// <summary>
        /// Process the one way message asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        private async void ProcessOneWay(
            IBrokeredMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            try
            {
                context.SetBrokeredMessage(message);
                await this.messageProcessor.ProcessAsync(message.Content, context, token).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Info(Strings.BrokeredMessageHandler_ProcessOneWay_Canceled);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.BrokeredMessageHandler_ProcessOneWay_Exception);
            }
        }

        /// <summary>
        /// Process the reply asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        private async void ProcessReply(
            IBrokeredMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            try
            {
                var broker = this.messageBrokerFactory.CreateExportedValue();
                await broker.ReplyReceivedAsync(message, context, token).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Info(Strings.BrokeredMessageHandler_ProcessAndReply_Canceled);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.BrokeredMessageHandler_ProcessAndReply_Exception);
            }
        }

        /// <summary>
        /// Process the message and respond asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        private async void ProcessAndRespond(
            IBrokeredMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            try
            {
                IMessage response = null;
                Exception exception = null;
                try
                {
                    context.SetBrokeredMessage(message);
                    response = await this.messageProcessor.ProcessAsync(message.Content, context, token).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    response = new ExceptionResponseMessage { Exception = new ExceptionData(exception) };
                }

                var broker = this.messageBrokerFactory.CreateExportedValue();
                var responseMessage = broker.CreateBrokeredMessageBuilder<BrokeredMessage>()
                    .ReplyTo(message.Id, message.Sender)
                    .WithContent(response)
                    .OneWay()
                    .BrokeredMessage;

                await broker.DispatchAsync(responseMessage, context, token).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Info(Strings.BrokeredMessageHandler_ProcessAndRespond_Canceled);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.BrokeredMessageHandler_ProcessAndRespond_Exception);
            }
        }
    }
}