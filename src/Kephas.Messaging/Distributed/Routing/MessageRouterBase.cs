// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouterBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message router base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for message routers.
    /// </summary>
    public abstract class MessageRouterBase : Loggable, IMessageRouter, IDisposable
    {
        private readonly IExportFactory<IBrokeredMessageBuilder> messageBuilderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterBase"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="messageBuilderFactory">The message builder factory.</param>
        protected MessageRouterBase(
            IMessageProcessor messageProcessor,
            IExportFactory<IBrokeredMessageBuilder> messageBuilderFactory)
        {
            this.messageBuilderFactory = messageBuilderFactory;
            this.MessageProcessor = messageProcessor;
        }

        /// <summary>
        /// Occurs when a reply for is received to match a request sent from the container message broker.
        /// </summary>
        public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        public IMessageProcessor MessageProcessor { get; }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <remarks>
        /// This method is invoked by the message broker when it identifies this router as handler for
        /// a brokered message. The router needs to send the message through the physical medium and,
        /// if necessary, prepare a response.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        public virtual async Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));
            Requires.NotNull(context, nameof(context));

            if (brokeredMessage.IsOneWay)
            {
                this.RouteOutputAsync(brokeredMessage, context, default)
                    .ContinueWith(
                        t => this.Logger.Warn(t.Exception, string.Format(Strings.MessageRouterBase_ProcessOneWay_Exception, brokeredMessage)),
                        TaskContinuationOptions.OnlyOnFaulted);
                return (RoutingInstruction.None, null);
            }

            (RoutingInstruction action, IMessage reply) result = default;
            Exception exception = null;
            try
            {
                result = await this.RouteOutputAsync(brokeredMessage, context, cancellationToken)
                       .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
            {
                return result;
            }

            return (RoutingInstruction.Reply, new ExceptionResponseMessage { Exception = new ExceptionData(exception) });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing).
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the
        /// MessageRouterBase and optionally releases the managed
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Creates a brokered messsage builder.
        /// </summary>
        /// <param name="context">The publishing context.</param>
        /// <returns>
        /// The new brokered messsage builder.
        /// </returns>
        protected virtual IBrokeredMessageBuilder CreateBrokeredMessageBuilder(IContext context)
        {
            Requires.NotNull(context, nameof(context));

            return this.messageBuilderFactory.CreateInitializedValue(context);
        }

        /// <summary>
        /// Routes the message received from the input queue asynchronously.
        /// </summary>
        /// <remarks>
        /// This method is called by the input queue when a message is received.
        /// For a reply, the router notifies the message broker, otherwise it calls the message processor
        /// to handle the message. Depending whether the message is one-way or not, a reply is routed through the
        /// output queue.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<(RoutingInstruction action, IMessage reply)> RouteInputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            try
            {
                if (brokeredMessage.ReplyToMessageId != null)
                {
                    this.OnReplyReceived(new ReplyReceivedEventArgs { Message = brokeredMessage, Context = context });
                    return (RoutingInstruction.None, null);
                }

                if (brokeredMessage.IsOneWay)
                {
                    // for one way or replies do not wait for a response
                    this.ProcessAsync(brokeredMessage, context, default)
                        .ContinueWith(
                            t => this.Logger.Error(t.Exception, Strings.MessageRouterBase_ErrorsOccurredWhileProcessingOneWay_Exception.FormatWith(brokeredMessage)),
                            TaskContinuationOptions.OnlyOnFaulted);
                    return (RoutingInstruction.None, null);
                }

                IMessage reply = null;
                try
                {
                    reply = await this.ProcessAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    reply = new ExceptionResponseMessage { Exception = new ExceptionData(ex) };
                }

                var replyMessage = this.CreateBrokeredMessageBuilder(context)
                    .ReplyTo(brokeredMessage)
                    .WithContent(reply)
                    .BrokeredMessage;

                return await this.RouteOutputAsync(replyMessage, context, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.MessageRouterBase_ErrorsOccurredWhileRoutingMessage_Exception.FormatWith(brokeredMessage));
                return (RoutingInstruction.None, null);
            }
        }

        /// <summary>
        /// Processes the brokered message locally, asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the reply message.
        /// </returns>
        protected virtual async Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            var response = await this.MessageProcessor.ProcessAsync(
                brokeredMessage.Content,
                ctx => ctx.Merge(context).SetBrokeredMessage(brokeredMessage),
                cancellationToken).PreserveThreadContext();
            return response;
        }

        /// <summary>
        /// Routes the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <remarks>
        /// The one-way handling is performed in the <see cref="DispatchAsync(IBrokeredMessage, IContext, CancellationToken)"/>
        /// method, here the message is purely routed through the transport medium.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected abstract Task<(RoutingInstruction action, IMessage reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Raises the reply received event.
        /// </summary>
        /// <param name="eventArgs">Event information to send to registered event handlers.</param>
        protected virtual void OnReplyReceived(ReplyReceivedEventArgs eventArgs)
        {
            this.ReplyReceived?.Invoke(this, eventArgs);
        }
    }
}
