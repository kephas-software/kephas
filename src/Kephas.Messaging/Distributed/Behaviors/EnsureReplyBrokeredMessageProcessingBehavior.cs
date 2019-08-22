// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureReplyBrokeredMessageProcessingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ensure reply brokered message processing behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Behaviors
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Behaviors.AttributedModel;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Processing behavior for distributed messaging ensuring that, in case of an exception,
    /// the sender receives an exception message.
    /// </summary>
    [MessageProcessingBehavior(MessageTypeMatching.TypeOrHierarchy)]
    [ProcessingPriority(Priority.Highest + 20)]
    public class EnsureReplyBrokeredMessageProcessingBehavior : MessageProcessingBehaviorBase<IBrokeredMessage>
    {
        /// <summary>
        /// The message broker factory.
        /// </summary>
        private readonly IExportFactory<IMessageBroker> messageBrokerFactory;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EnsureReplyBrokeredMessageProcessingBehavior"/> class.
        /// </summary>
        /// <param name="messageBrokerFactory">The message broker factory.</param>
        public EnsureReplyBrokeredMessageProcessingBehavior(IExportFactory<IMessageBroker> messageBrokerFactory)
        {
            this.messageBrokerFactory = messageBrokerFactory;
        }

        /// <summary>
        /// Interception called after invoking the handler to process the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        /// <remarks>
        /// The context will contain the response returned by the handler.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        public override async Task AfterProcessAsync(IBrokeredMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            // do not reply for one way messages, nor when there is no exception
            if (context.Exception == null || message.IsOneWay)
            {
                return;
            }

            Task.Factory.StartNew(() => this.RespondException(message, context, context.Exception, token));
        }

        /// <summary>
        /// Respond asynchronously to the message with an exception.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="token">The cancellation token.</param>
        private async void RespondException(
            IBrokeredMessage message,
            IMessageProcessingContext context,
            Exception exception,
            CancellationToken token)
        {
            try
            {
                var response = new ExceptionResponseMessage { Exception = new ExceptionData(exception) };

                var broker = this.messageBrokerFactory.CreateExportedValue();
                var responseMessage = broker.CreateBrokeredMessageBuilder(context)
                    .ReplyTo(message)
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