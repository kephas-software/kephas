// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplexRouterBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the duplex router base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Text;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for routers with input and output queues.
    /// </summary>
    public abstract class DuplexMessageRouterBase : MessageRouterBase
    {
        private readonly Lazy<IMessageBroker> lazyMessageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexMessageRouterBase"/> class.
        /// </summary>
        /// <param name="lazyMessageBroker">The lazy message broker.</param>
        public DuplexMessageRouterBase(Lazy<IMessageBroker> lazyMessageBroker)
        {
            this.lazyMessageBroker = lazyMessageBroker;
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium (core implementation).
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override async Task<(RoutingInstruction action, IMessage reply)> SendCoreAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            await this.RouteOutputAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();

            return (RoutingInstruction.None, null);
        }

        /// <summary>
        /// Routes the message received from the input queue asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task RouteInputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            try
            {
                var messageBroker = this.lazyMessageBroker.Value;

                if (brokeredMessage.ReplyToMessageId != null)
                {
                    this.OnReplyReceived(new ReplyReceivedEventArgs { Message = brokeredMessage, Context = context });
                    return;
                }

                if (brokeredMessage.IsOneWay)
                {
                    // for one way or replies do not wait for a response
                    messageBroker.DispatchAsync(brokeredMessage, context)
                        .ContinueWith(
                            t => this.Logger.Error(t.Exception, Strings.DuplexRouterBase_ErrorsOccurredDuringDispatching_Exception),
                            TaskContinuationOptions.OnlyOnFaulted);
                    return;
                }

                var reply = await messageBroker.DispatchAsync(brokeredMessage, context).PreserveThreadContext();
                var replyMessage = messageBroker.CreateBrokeredMessageBuilder(context)
                    .ReplyTo(brokeredMessage)
                    .WithContent(reply)
                    .BrokeredMessage;

                await this.RouteOutputAsync(replyMessage, context, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.DuplexRouterBase_ErrorsOccurredWhileRoutingMessage_Exception.FormatWith(brokeredMessage));
            }
        }

        /// <summary>
        /// Routes the message to the output queue asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected abstract Task RouteOutputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken);
    }
}
