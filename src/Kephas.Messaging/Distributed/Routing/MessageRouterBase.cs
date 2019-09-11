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
    using Kephas.Diagnostics.Contracts;
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
        /// <summary>
        /// Occurs when a reply is received.
        /// </summary>
        public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        public virtual async Task<(RoutingInstruction action, IMessage reply)> SendAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));
            Requires.NotNull(context, nameof(context));

            if (brokeredMessage.IsOneWay)
            {
                this.ProcessAsync(brokeredMessage, context, default)
                    .ContinueWith(
                        t => this.Logger.Warn(t.Exception, string.Format(Strings.MessageRouterBase_ProcessOneWay_Exception, brokeredMessage)),
                        TaskContinuationOptions.OnlyOnFaulted);
                return (RoutingInstruction.None, null);
            }

            IMessage reply = null;
            Exception exception = null;
            try
            {
                reply = await this.ProcessAsync(brokeredMessage, context, cancellationToken)
                       .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                reply = new ExceptionResponseMessage { Exception = new ExceptionData(exception) };
            }

            return (RoutingInstruction.Reply, reply);
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
        protected abstract Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken);

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
