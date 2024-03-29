﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for message routers.
    /// </summary>
    public abstract class MessageRouterBase : Loggable, IMessageRouter, IAsyncInitializable, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouterBase"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        protected MessageRouterBase(
            IInjectableFactory injectableFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor)
            : base(injectableFactory)
        {
            this.InjectableFactory = injectableFactory;
            this.AppRuntime = appRuntime;
            this.MessageProcessor = messageProcessor;

            this.InitializationMonitor = new InitializationMonitor<IMessageRouter>(this.GetType());
            this.FinalizationMonitor = new FinalizationMonitor<IMessageRouter>(this.GetType());
        }

        /// <summary>
        /// Occurs when a reply for is received to match a request sent from the container message broker.
        /// </summary>
        public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IInjectableFactory InjectableFactory { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        public IMessageProcessor MessageProcessor { get; }

        /// <summary>
        /// Gets the application context.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IContext AppContext { get; private set; }

        /// <summary>
        /// Gets the initialization monitor.
        /// </summary>
        /// <value>
        /// The initialization monitor.
        /// </value>
        protected InitializationMonitor<IMessageRouter> InitializationMonitor { get; }

        /// <summary>
        /// Gets the finalization monitor.
        /// </summary>
        /// <value>
        /// The finalization monitor.
        /// </value>
        protected FinalizationMonitor<IMessageRouter> FinalizationMonitor { get; }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task InitializeAsync(IContext? context, CancellationToken cancellationToken = default)
        {
            this.InitializationMonitor.AssertIsNotStarted();

            var messageRouterName = this.GetType().Name;
            this.Logger.Info("Starting the {router} message router...", messageRouterName);

            this.AppContext = context!;

            this.InitializationMonitor.Start();

            try
            {
                await this.InitializeCoreAsync(context, cancellationToken).PreserveThreadContext();

                this.InitializationMonitor.Complete();

                this.Logger.Info("{router} started.", messageRouterName);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "{router} failed to initialize.", messageRouterName);
                this.InitializationMonitor.Fault(ex);
                throw;
            }
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <remarks>
        /// This method is invoked by the message broker when it identifies this router as handler for
        /// a brokered message. The router needs to send the message through the physical medium and,
        /// if necessary, prepare a response.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        public virtual async Task<(RoutingInstruction action, object? reply)> DispatchAsync(
            IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
        {
            brokeredMessage = brokeredMessage ?? throw new System.ArgumentNullException(nameof(brokeredMessage));
            context = context ?? throw new ArgumentNullException(nameof(context));

            brokeredMessage.TraceOutputRoute(this, this.AppRuntime.GetAppInstanceId());
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Routing message {message} through: {messageTrace}.", brokeredMessage, brokeredMessage.Trace);
            }

            if (brokeredMessage.IsOneWay)
            {
                this.RouteOutputAsync(brokeredMessage, context, default)
                    .ContinueWith(
                        t => this.Logger.Warn(t.Exception, string.Format(Strings.MessageRouterBase_ProcessOneWay_Exception, brokeredMessage)),
                        TaskContinuationOptions.OnlyOnFaulted);
                return (RoutingInstruction.None, null);
            }

            (RoutingInstruction action, object? reply) result = default;
            Exception? exception = null;
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

            return (RoutingInstruction.Reply, new ExceptionResponse { Exception = new ExceptionData(exception) });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing).
            if (this.InitializationMonitor.IsNotStarted)
            {
                return;
            }

            if (this.FinalizationMonitor.IsCompleted)
            {
                return;
            }

            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            var messageRouterName = this.GetType().Name;
            try
            {
                this.Logger.Info("Stopping the {router} message router...", messageRouterName);

                this.FinalizationMonitor.Start();

                this.Dispose(true);

                this.FinalizationMonitor.Complete();
                this.Logger.Info("{router} message router stopped.", messageRouterName);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "{router} failed to stop.", messageRouterName);
                this.FinalizationMonitor.Fault(ex);
                throw;
            }
            finally
            {
                this.InitializationMonitor.Reset();
                GC.SuppressFinalize(this);
            }
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
        /// Actual initialization of the router.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task InitializeCoreAsync(IContext? context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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
        /// <param name="context">The input context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<(RoutingInstruction action, object? reply)> RouteInputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            try
            {
                var appId = this.AppRuntime.GetAppId();
                var appInstanceId = this.AppRuntime.GetAppInstanceId();

                brokeredMessage.TraceInputRoute(this, appInstanceId);
                if (this.Logger.IsTraceEnabled())
                {
                    this.Logger.Trace("Routing message {message} through: {messageTrace}.", brokeredMessage, brokeredMessage.Trace);
                }

                // if the input queue notifies a reply, notify it further to the message broker.
                if (brokeredMessage.ReplyTo != null)
                {
                    this.OnReplyReceived(new ReplyReceivedEventArgs { Message = brokeredMessage, Context = context });
                    return (RoutingInstruction.None, null);
                }

                // identify the local and the remote recipients
                var localRecipients = brokeredMessage.Recipients?
                    .Where(r => (r.AppInstanceId == null || r.AppInstanceId == appInstanceId)
                                    && (r.AppId == null || r.AppId == appId))
                    .ToList();
                var remoteRecipients = brokeredMessage.Recipients?
                    .Where(r => (r.AppInstanceId != null && r.AppInstanceId != appInstanceId)
                                    || (r.AppId != null && r.AppId != appId))
                    .ToList();

                // for remote recipients redirect the message to the output.
                var remoteInstruction = RoutingInstruction.None;
                object? remoteReply = null;
                if (remoteRecipients?.Any() ?? false)
                {
                    var remoteMessage = !localRecipients.Any()
                        ? brokeredMessage
                        : brokeredMessage.Clone(remoteRecipients);
                    using var redirectContext = this.InjectableFactory.Create<DispatchingContext>(remoteMessage);
                    redirectContext.Impersonate(context);

                    remoteMessage.TraceOutputRoute(this, appInstanceId);
                    if (this.Logger.IsTraceEnabled())
                    {
                        this.Logger.Trace("Routing message {message} through: {messageTrace}.", remoteMessage, remoteMessage.Trace);
                    }

                    (remoteInstruction, remoteReply) = await this.RouteOutputAsync(remoteMessage, redirectContext, cancellationToken).PreserveThreadContext();
                }

                // for local recipients, handle the request here
                var localInstruction = RoutingInstruction.None;
                object? localReply = null;
                if ((localRecipients?.Any() ?? false) || !(brokeredMessage.Recipients?.Any() ?? false))
                {
                    var localMessage = brokeredMessage.Recipients == null || !(remoteRecipients?.Any() ?? false)
                        ? brokeredMessage
                        : brokeredMessage.Clone(localRecipients);
                    if (localMessage.IsOneWay)
                    {
                        // for one way or replies do not wait for a response
                        this.ProcessAsync(localMessage, context, default)
                            .ContinueWith(
                                t => this.Logger.Error(t.Exception, Strings.MessageRouterBase_ErrorsOccurredWhileProcessingOneWay_Exception.FormatWith(brokeredMessage)),
                                TaskContinuationOptions.OnlyOnFaulted);
                    }
                    else
                    {
                        object? reply = null;
                        try
                        {
                            reply = await this.ProcessAsync(localMessage, context, cancellationToken).PreserveThreadContext();
                        }
                        catch (Exception ex)
                        {
                            reply = new ExceptionResponse { Exception = new ExceptionData(ex) };
                        }

                        // after processing requests expecting an answer, redirect the reply
                        // through the same infrastructure back to caller.
                        using var replyContext = this.InjectableFactory.Create<DispatchingContext>(reply);
                        replyContext.Impersonate(context).ReplyTo(brokeredMessage);

                        replyContext.BrokeredMessage.TraceOutputRoute(this, appInstanceId);
                        if (this.Logger.IsTraceEnabled())
                        {
                            this.Logger.Trace("Routing message {message} through: {messageTrace}.", replyContext.BrokeredMessage, replyContext.BrokeredMessage.Trace);
                        }

                        (localInstruction, localReply) = await this.RouteOutputAsync(replyContext.BrokeredMessage, replyContext, cancellationToken).PreserveThreadContext();
                    }
                }

                if (remoteInstruction == RoutingInstruction.Reply)
                {
                    if (localInstruction == RoutingInstruction.Reply)
                    {
                        throw new RoutingException(Strings.MessageRouterBase_RouteInputAsync_CannotHandleMultipleReplies.FormatWith(brokeredMessage));
                    }

                    return (remoteInstruction, remoteReply);
                }

                return (localInstruction, localReply);
            }
            catch (RoutingException ex)
            {
                this.Logger.Error(ex, Strings.MessageRouterBase_ErrorsOccurredWhileRoutingMessage_Exception.FormatWith(brokeredMessage));
                throw;
            }
            catch (Exception ex)
            {
                var routingException = new RoutingException(
                    Strings.MessageRouterBase_ErrorsOccurredWhileRoutingMessage_Exception.FormatWith(brokeredMessage),
                    ex);
                this.Logger.Error(ex, Strings.MessageRouterBase_ErrorsOccurredWhileRoutingMessage_Exception.FormatWith(brokeredMessage));
                throw routingException;
            }
        }

        /// <summary>
        /// Processes the brokered message locally, asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the reply message.
        /// </returns>
        protected virtual async Task<object?> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
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
        /// The one-way handling is performed in the <see cref="DispatchAsync(IBrokeredMessage, IDispatchingContext, CancellationToken)"/>
        /// method, here the message is purely routed through the transport medium.
        /// </remarks>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected abstract Task<(RoutingInstruction action, object? reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken);

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
