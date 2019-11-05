// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageRouter.cs" company="Kephas Software SRL">
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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An in process message router invoking the message processor.
    /// </summary>
    /// <remarks>
    /// For the in-process message router, the <see cref="DispatchAsync"/> method represents the input queue.
    /// </remarks>
    [ProcessingPriority(Priority.Lowest)]
    [MessageRouter(IsFallback = true)]
    public class InProcessMessageRouter : MessageRouterBase, IInitializable
    {
        private const string ChannelType = Endpoint.AppScheme;

        private static readonly ConcurrentDictionary<string, MessageQueue> Channels = new ConcurrentDictionary<string, MessageQueue>();

        private readonly InitializationMonitor<InProcessMessageRouter> initializationMonitor;
        private readonly FinalizationMonitor<InProcessMessageRouter> finalizationMonitor;

        private IContext appContext;
        private MessageQueue messageQueue;
        private MessageQueue appMessageQueue;
        private MessageQueue appInstanceMessageQueue;
        private string rootChannelName;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        public InProcessMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor)
            : base(contextFactory, messageProcessor)
        {
            this.AppRuntime = appRuntime;

            this.initializationMonitor = new InitializationMonitor<InProcessMessageRouter>(this.GetType());
            this.finalizationMonitor = new FinalizationMonitor<InProcessMessageRouter>(this.GetType());
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        public virtual void Initialize(IContext context = null)
        {
            this.initializationMonitor.AssertIsNotStarted();

            var messageRouterName = this.GetType().Name;
            this.Logger.Info($"Starting the {messageRouterName} message router...");

            this.initializationMonitor.Start();

            this.rootChannelName = ChannelType;
            this.messageQueue = Channels.GetOrAdd(this.rootChannelName, _ => new MessageQueue(this.rootChannelName));
            this.messageQueue.MessageArrived += this.HandleMessageArrivedAsync;

            var appChannelName = $"{this.rootChannelName}:{this.AppRuntime.GetAppId()}";
            this.appMessageQueue = Channels.GetOrAdd(appChannelName, _ => new MessageQueue(appChannelName));
            this.appMessageQueue.MessageArrived += this.HandleMessageArrivedAsync;

            var appInstanceChannelName = $"{this.rootChannelName}:{this.AppRuntime.GetAppInstanceId()}";
            this.appInstanceMessageQueue = Channels.GetOrAdd(appInstanceChannelName, _ => new MessageQueue(appInstanceChannelName));
            this.appInstanceMessageQueue.MessageArrived += this.HandleMessageArrivedAsync;

            this.Logger.Info($"{messageRouterName} started.");

            this.initializationMonitor.Complete();
        }

        /// <summary>
        /// Handles the message arrived event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Message event information.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task HandleMessageArrivedAsync(object sender, MessageEventArgs e)
        {
            try
            {
                var brokeredMessage = e.Message;
                await this.RouteInputAsync(brokeredMessage, this.appContext, default).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, $"Error while handling message '{e.Message}'.");
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.initializationMonitor.IsNotStarted)
            {
                return;
            }

            this.FinalizeCore();
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
        protected override Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<IMessage>();

            // make processing really async for in-process handling
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        var result = await base.ProcessAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
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
        /// Routes the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override async Task<(RoutingInstruction action, IMessage reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
        {
            this.initializationMonitor.AssertIsCompletedSuccessfully();

            if (brokeredMessage.Recipients?.Any() ?? false)
            {
                foreach (var recipient in brokeredMessage.Recipients)
                {
                    var channelName = string.IsNullOrEmpty(recipient.AppInstanceId)
                                        ? string.IsNullOrEmpty(recipient.AppId)
                                            ? this.rootChannelName
                                            : $"{this.rootChannelName}:{recipient.AppId}"
                                        : $"{this.rootChannelName}:{recipient.AppInstanceId}";
                    await this.PublishAsync(brokeredMessage, channelName).PreserveThreadContext();
                }
            }
            else
            {
                await this.PublishAsync(brokeredMessage, this.rootChannelName).PreserveThreadContext();
            }

            return (RoutingInstruction.None, null);
        }

        private async Task PublishAsync(IBrokeredMessage message, string channelName)
        {
            var queue = Channels.GetOrAdd(channelName, _ => new MessageQueue(channelName));
            await queue.PublishAsync(message).PreserveThreadContext();
        }

        private void FinalizeCore()
        {
            if (this.finalizationMonitor.IsCompleted)
            {
                return;
            }

            this.initializationMonitor.AssertIsCompletedSuccessfully();

            var messageRouterName = this.GetType().Name;
            this.Logger.Info($"Stopping the {messageRouterName} message router...");

            this.finalizationMonitor.Start();
            try
            {
                this.messageQueue.MessageArrived -= this.HandleMessageArrivedAsync;
                this.appMessageQueue.MessageArrived -= this.HandleMessageArrivedAsync;
                this.appInstanceMessageQueue.MessageArrived -= this.HandleMessageArrivedAsync;

                this.finalizationMonitor.Complete();
                this.Logger.Info($"{messageRouterName} message router stopped.");
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, $"{messageRouterName} failed to stop.");
                this.finalizationMonitor.Fault(ex);
                throw;
            }
            finally
            {
                this.initializationMonitor.Reset();
            }
        }

        /// <summary>
        /// Additional information for message events.
        /// </summary>
        protected class MessageEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public MessageEventArgs(IBrokeredMessage message)
            {
                this.Message = message;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            /// <value>
            /// The message.
            /// </value>
            public IBrokeredMessage Message { get; }
        }

        private delegate Task AsyncEventHandler<T>(object sender, T eventArgs)
            where T : EventArgs;

        private class MessageQueue : Loggable
        {
            private ConcurrentQueue<IBrokeredMessage> internalQueue = new ConcurrentQueue<IBrokeredMessage>();

            public MessageQueue(string channel)
            {
                this.Channel = channel;
            }

            public event AsyncEventHandler<MessageEventArgs> MessageArrived;

            public string Channel { get; }

            public Task PublishAsync(IBrokeredMessage message)
            {
                this.internalQueue.Enqueue(message);
                Task.Run(() =>
                {
                    if (!this.internalQueue.TryDequeue(out var msg))
                    {
                        return;
                    }

                    try
                    {
                        this.MessageArrived.Invoke(this, new MessageEventArgs(msg));
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex, $"Error while publishing message {msg}.");
                    }
                });

                return TaskHelper.CompletedTask;
            }
        }
    }
}