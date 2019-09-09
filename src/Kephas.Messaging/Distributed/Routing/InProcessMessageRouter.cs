// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null distributed message broker class.
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
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An in process message router invoking the message processor.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    [MessageRouter(IsFallback = true)]
    public class InProcessMessageRouter : MessageRouterBase
    {
        private readonly IMessageProcessor messageProcessor;
        private readonly IExportFactory<IBrokeredMessageBuilder> messageBuilderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessMessageRouter"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="messageBuilderFactory">The message builder factory.</param>
        public InProcessMessageRouter(IMessageProcessor messageProcessor, IExportFactory<IBrokeredMessageBuilder> messageBuilderFactory)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));
            Requires.NotNull(messageBuilderFactory, nameof(messageBuilderFactory));

            this.messageProcessor = messageProcessor;
            this.messageBuilderFactory = messageBuilderFactory;
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override Task SendAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));
            Requires.NotNull(context, nameof(context));

            return Task.Factory.StartNew(
                async () =>
                {

                    if (brokeredMessage.ReplyToMessageId != null)
                    {
                        this.OnReplyReceived(new ReplyReceivedEventArgs { Message = brokeredMessage, Context = context });
                    }
                    else if (brokeredMessage.IsOneWay)
                    {
                        this.ProcessOneWay(brokeredMessage, context);
                    }
                    else
                    {
                        await this.ProcessAndRespondAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
                    }
                },
                cancellationToken);
        }

        private async Task ProcessAndRespondAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            IMessage response = null;
            Exception exception = null;
            try
            {
                using (var messagingContext = context == null
                                                  ? new MessageProcessingContext(this.messageProcessor, brokeredMessage.Content)
                                                  : new MessageProcessingContext(context, this.messageProcessor, brokeredMessage.Content))
                {
                    messagingContext.SetBrokeredMessage(brokeredMessage);
                    response = await this.messageProcessor.ProcessAsync(brokeredMessage.Content, messagingContext, cancellationToken)
                           .PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                response = new ExceptionResponseMessage { Exception = new ExceptionData(exception) };
            }

            var builder = this.messageBuilderFactory.CreateExportedValue(context);
            var responseMessage = builder
                .ReplyTo(brokeredMessage.Id, brokeredMessage.Sender)
                .WithContent(response)
                .OneWay()
                .BrokeredMessage;

            this.OnReplyReceived(new ReplyReceivedEventArgs
            {
                Message = responseMessage,
                Context = context,
            });
        }

        private async void ProcessOneWay(IBrokeredMessage brokeredMessage, IContext context)
        {
            try
            {
                using (var messagingContext = context == null
                                                  ? new MessageProcessingContext(this.messageProcessor, brokeredMessage.Content)
                                                  : new MessageProcessingContext(context, this.messageProcessor, brokeredMessage.Content))
                {
                    messagingContext.SetBrokeredMessage(brokeredMessage);
                    await this.messageProcessor.ProcessAsync(brokeredMessage.Content, messagingContext).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, Strings.InProcessMessageRouter_MessageProcessor_Async_Exception);
            }
        }
    }
}