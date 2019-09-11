// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null distributed message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessMessageRouter"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        public InProcessMessageRouter(IMessageProcessor messageProcessor)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));

            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        public override Task<(RoutingInstruction action, IMessage reply)> SendAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));
            Requires.NotNull(context, nameof(context));

            if (brokeredMessage.ReplyToMessageId != null)
            {
                this.OnReplyReceived(new ReplyReceivedEventArgs { Message = brokeredMessage, Context = context });
                return Task.FromResult((action: RoutingInstruction.None, reply: (IMessage)null));
            }

            var completionSource = new TaskCompletionSource<(RoutingInstruction action, IMessage reply)>();

            // make processing really async
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        var result = await base.SendAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
                        completionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        completionSource.SetException(ex);
                    }
                },
                cancellationToken);

            return completionSource.Task;
        }

        /// <summary>
        /// Processes the message asynchronously.
        /// </summary>
        /// <remarks>
        /// The one-way handling is performed in the <see cref="SendAsync(IBrokeredMessage, IContext, CancellationToken)"/>
        /// method, here is handled purely the message over the transport medium.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override async Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            using (var messagingContext = context == null
                                              ? new MessageProcessingContext(this.messageProcessor)
                                              : new MessageProcessingContext(context, this.messageProcessor))
            {
                messagingContext.SetBrokeredMessage(brokeredMessage);
                return await this.messageProcessor.ProcessAsync(brokeredMessage.Content, messagingContext, cancellationToken)
                       .PreserveThreadContext();
            }
        }
    }
}