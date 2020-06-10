// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessAppMessageRouter.cs" company="Kephas Software SRL">
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
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An in process message router invoking the message processor.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class InProcessAppMessageRouter : MessageRouterBase
    {
        /// <summary>
        /// The channel type handled by the <see cref="InProcessAppMessageRouter"/>.
        /// </summary>
        public const string ChannelType = Endpoint.AppScheme;

        private static readonly ConcurrentDictionary<string, MessageQueue> Channels = new ConcurrentDictionary<string, MessageQueue>();

        private MessageQueue messageQueue;
        private MessageQueue appMessageQueue;
        private MessageQueue appInstanceMessageQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        public InProcessAppMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor)
            : base(contextFactory, appRuntime, messageProcessor)
        {
        }

        /// <summary>
        /// Gets the name of the root channel.
        /// </summary>
        /// <value>
        /// The name of the root channel.
        /// </value>
        protected string RootChannelName { get; private set; }

        /// <summary>
        /// Actual initialization of the router.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override async Task InitializeCoreAsync(IContext context, CancellationToken cancellationToken)
        {
            this.RootChannelName = this.ComputeRootChannelName();
            this.messageQueue = Channels.GetOrAdd(this.RootChannelName, _ => new MessageQueue(this.ContextFactory, this.RootChannelName));
            this.messageQueue.MessageArrived += this.HandleMessageArrivedAsync;

            var appChannelName = $"{this.RootChannelName}:{this.AppRuntime.GetAppId()}";
            this.appMessageQueue = Channels.GetOrAdd(appChannelName, _ => new MessageQueue(this.ContextFactory, appChannelName));
            this.appMessageQueue.MessageArrived += this.HandleMessageArrivedAsync;

            var appInstanceChannelName = $"{this.RootChannelName}:{this.AppRuntime.GetAppInstanceId()}";
            this.appInstanceMessageQueue = Channels.GetOrAdd(appInstanceChannelName, _ => new MessageQueue(this.ContextFactory, appInstanceChannelName));
            this.appInstanceMessageQueue.MessageArrived += this.HandleMessageArrivedAsync;
        }

        /// <summary>
        /// Calculates the root channel name.
        /// </summary>
        /// <returns>
        /// The calculated root channel name.
        /// </returns>
        protected virtual string ComputeRootChannelName() => ChannelType;

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
                await this.RouteInputAsync(brokeredMessage, this.AppContext, default).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while handling message '{message}'.", e.Message);
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
        protected override async Task<(RoutingInstruction action, IMessage? reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            if (brokeredMessage.Recipients?.Any() ?? false)
            {
                var groups = brokeredMessage.Recipients
                    .GroupBy(r => this.GetChannelName(r))
                    .Select(g => (channelName: g.Key, recipients: g))
                    .ToList();

                if (groups.Count == 1)
                {
                    await this.PublishAsync(brokeredMessage, groups[0].channelName).PreserveThreadContext();
                }
                else
                {
                    foreach (var (channelName, recipients) in groups)
                    {
                        await this.PublishAsync(brokeredMessage.Clone(recipients.ToList()), channelName).PreserveThreadContext();
                    }
                }
            }
            else
            {
                await this.PublishAsync(brokeredMessage, this.RootChannelName).PreserveThreadContext();
            }

            return (RoutingInstruction.None, null);
        }

        /// <summary>
        /// Gets the channel name for the provided recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <returns>
        /// The channel name.
        /// </returns>
        protected virtual string GetChannelName(IEndpoint recipient)
        {
            return string.IsNullOrEmpty(recipient.AppInstanceId)
                        ? string.IsNullOrEmpty(recipient.AppId)
                            ? this.RootChannelName
                            : $"{this.RootChannelName}:{recipient.AppId}"
                        : $"{this.RootChannelName}:{recipient.AppInstanceId}";
        }

        /// <summary>
        /// Releases the unmanaged resources used by the MessageRouterBase and optionally releases the
        /// managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.messageQueue.MessageArrived -= this.HandleMessageArrivedAsync;
            this.appMessageQueue.MessageArrived -= this.HandleMessageArrivedAsync;
            this.appInstanceMessageQueue.MessageArrived -= this.HandleMessageArrivedAsync;

            base.Dispose(disposing);
        }

        private async Task PublishAsync(IBrokeredMessage message, string channelName)
        {
            var queue = Channels.GetOrAdd(channelName, _ => new MessageQueue(this.ContextFactory, channelName));
            await queue.PublishAsync(message).PreserveThreadContext();
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

            public MessageQueue(IContextFactory contextFactory, string channel)
                : base(contextFactory)
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
                        this.MessageArrived?.Invoke(this, new MessageEventArgs(msg));
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex, $"Error while issuing the {nameof(this.MessageArrived)} event for message {{message}}.", msg);
                    }
                });

                return Task.CompletedTask;
            }
        }
    }
}