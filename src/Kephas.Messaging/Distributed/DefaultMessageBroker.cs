// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Distributed.Routing.Composition;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a <see cref="IMessageBroker"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageBroker : Loggable, IMessageBroker, IAsyncInitializable, IAsyncFinalizable
    {
        /// <summary>
        /// The dictionary for message synchronization.
        /// </summary>
        private readonly
            ConcurrentDictionary<string, (CancellationTokenSource cancellationTokenSource,
                TaskCompletionSource<IMessage> taskCompletionSource)> messageSyncDictionary =
                new ConcurrentDictionary<string, (CancellationTokenSource, TaskCompletionSource<IMessage>)>();

        private readonly ICollection<IExportFactory<IMessageRouter, MessageRouterMetadata>> routerFactories;
        private readonly IExportFactory<IBrokeredMessageBuilder> builderFactory;
        private readonly InitializationMonitor<IMessageBroker> initMonitor;
        private ICollection<(Regex regex, string channel, bool isFallback, IMessageRouter router)> routerMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageBroker"/> class.
        /// </summary>
        /// <param name="routerFactories">The router factories.</param>
        /// <param name="builderFactory">The builder factory.</param>
        public DefaultMessageBroker(
            ICollection<IExportFactory<IMessageRouter, MessageRouterMetadata>> routerFactories,
            IExportFactory<IBrokeredMessageBuilder> builderFactory)
        {
            this.initMonitor = new InitializationMonitor<IMessageBroker>(this.GetType());
            this.routerFactories = routerFactories;
            this.builderFactory = builderFactory;
        }

        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">Optional. The dispatching context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        public virtual Task<IMessage> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));
            Requires.NotNull(context, nameof(context));

            this.initMonitor.AssertIsCompletedSuccessfully();

            if (brokeredMessage.IsOneWay)
            {
                this.LogBeforeSend(brokeredMessage);
                this.SendAsync(brokeredMessage, context, cancellationToken)
                    .ContinueWith(
                        t => this.Logger.Error(string.Format(Strings.DefaultMessageBroker_ErrorsOccurredWhileSending_Exception, brokeredMessage)),
                        TaskContinuationOptions.OnlyOnFaulted);
                return Task.FromResult((IMessage)null);
            }

            var completionSource = this.GetTaskCompletionSource(brokeredMessage);

            this.LogBeforeSend(brokeredMessage);
            this.SendAsync(brokeredMessage, context, cancellationToken)
                .ContinueWith(
                    t => this.Logger.Error(string.Format(Strings.DefaultMessageBroker_ErrorsOccurredWhileSending_Exception, brokeredMessage)),
                    TaskContinuationOptions.OnlyOnFaulted);

            // Returns an awaiter for the answer, must pair with the original message ID.
            return completionSource.Task;
        }

        /// <summary>
        /// Creates a brokered messsage builder.
        /// </summary>
        /// <param name="context">The publishing context.</param>
        /// <returns>
        /// The new brokered messsage builder.
        /// </returns>
        public IBrokeredMessageBuilder CreateBrokeredMessageBuilder(IContext context)
        {
            Requires.NotNull(context, nameof(context));

            return this.builderFactory.CreateExportedValue(context);
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public virtual async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.initMonitor.Start();

            var asyncRouterMap = this.routerFactories
                .Order()
                .Select(f => (
                    regex: string.IsNullOrEmpty(f.Metadata.ReceiverUrlRegex) ? null : new Regex(f.Metadata.ReceiverUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled),
                    channel: f.Metadata.Channel,
                    isFallback: f.Metadata.IsFallback,
                    asyncRouter: this.TryCreateRouterAsync(f, context)))
                .ToList();

            await Task.WhenAll(asyncRouterMap.Select(m => m.asyncRouter)).PreserveThreadContext();

            this.routerMap = asyncRouterMap
                                .Where(m => m.asyncRouter.Result != null)
                                .Select(m => (m.regex, m.channel, m.isFallback, m.asyncRouter.Result))
                                .ToList();
            foreach (var map in this.routerMap)
            {
                map.router.ReplyReceived += this.HandleReplyReceived;
            }

            this.initMonitor.Complete();
        }

        private async Task<IMessageRouter> TryCreateRouterAsync(IExportFactory<IMessageRouter, MessageRouterMetadata> f, IContext context)
        {
            try
            {
                return await f.CreateExportedValueAsync(context).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, $"Error while trying to create and initialize router '{f.Metadata.AppServiceImplementationType}'.");
                if (f.Metadata.IsOptional)
                {
                    this.Logger.Warn($"Router '{f.Metadata.AppServiceImplementationType}' will be ignored.");
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual async Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            foreach (var map in this.routerMap)
            {
                map.router.ReplyReceived -= this.HandleReplyReceived;
                if (map.router is IAsyncFinalizable asyncFinRouter)
                {
                    await asyncFinRouter.FinalizeAsync(context, cancellationToken).PreserveThreadContext();
                }
                else if (map.router is IFinalizable finRouter)
                {
                    finRouter.Finalize(context);
                }
                else if (map.router is IDisposable disposableRouter)
                {
                    disposableRouter.Dispose();
                }
            }

            this.initMonitor.Reset();
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
        protected virtual async Task SendAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken)
        {
            var results = await this.CollectSendResultsAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
            var replies = results
                .Where(t => t.action == RoutingInstruction.Reply)
                .Select(t => t.reply)
                .Distinct() // make from multiple nulls one.
                .ToList();

            if (replies.Count == 0)
            {
                return;
            }

            var responseMessage = this.GetResponseMessage(
                replies.Count == 1 ? replies[0] : new AggregateMessage { Messages = replies },
                brokeredMessage,
                context);

            await this.SendAsync(responseMessage, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Notification method for receiving a reply.
        /// </summary>
        /// <param name="replyMessage">Message describing the reply.</param>
        /// <param name="context">Optional. The reply context.</param>
        protected virtual void ReceiveReply(
            IBrokeredMessage replyMessage,
            IContext context = null)
        {
            var replyToMessageId = replyMessage.ReplyToMessageId;
            if (string.IsNullOrEmpty(replyToMessageId))
            {
                this.Logger.Warn(Strings.MessageBrokerBase_MissingReplyToMessageId_Exception, nameof(IBrokeredMessage.ReplyToMessageId), replyMessage.Content);
                return;
            }

            if (!this.messageSyncDictionary.TryRemove(replyToMessageId, out var syncEntry))
            {
                this.Logger.Warn(Strings.MessageBrokerBase_ReplyToMessageNotFound_Exception, replyToMessageId, replyMessage.Content);
                return;
            }

            this.LogOnReceive(replyMessage);

            syncEntry.cancellationTokenSource.Dispose();

            if (replyMessage.Content is ExceptionResponseMessage exceptionMessage)
            {
                var exception = new MessagingException(exceptionMessage.Exception);
                syncEntry.taskCompletionSource.SetException(exception);
            }
            else
            {
                syncEntry.taskCompletionSource.SetResult(replyMessage.Content);
            }
        }

        /// <summary>
        /// Handles the reply received event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event information.</param>
        private void HandleReplyReceived(object sender, ReplyReceivedEventArgs e)
        {
            this.ReceiveReply(e.Message, e.Context);
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields an action and a reply.
        /// </returns>
        private async Task<ICollection<(RoutingInstruction action, IMessage reply)>> CollectSendResultsAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken)
        {
            if (brokeredMessage.Recipients == null || !brokeredMessage.Recipients.Any())
            {
                var router = this.routerMap.FirstOrDefault(f => f.isFallback).router;
                if (router != null)
                {
                    return new[] { await router.SendAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext() };
                }

                throw new MessagingException(Strings.DefaultMessageBroker_CannotHandleMessagesWithoutRecipients_Exception);
            }

            var recipientMappings = brokeredMessage.Recipients
                .Select(r => (recipient: r, router: this.routerMap.FirstOrDefault(f => f.isFallback || (f.regex?.IsMatch(r.Url.ToString()) ?? false) || (f.channel != null && f.channel == brokeredMessage.Channel)).router))
                .ToList();
            var unhandledRecipients = recipientMappings
                .Where(c => c.router == null)
                .Select(m => m.recipient)
                .ToList();
            if (unhandledRecipients.Count > 0)
            {
                throw new MessagingException(string.Format(Strings.DefaultMessageBroker_NoMessageRoutersCanHandleRecipients_Exception, string.Join(", ", unhandledRecipients)));
            }

            // optimization for the typical case when there is only one router to handle the recipients.
            if (recipientMappings.Count == 1)
            {
                var router = recipientMappings[0].router;
                return new[] { await router.SendAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext() };
            }

            // general case when there are more routers for the recipients
            var routerMappings = recipientMappings
                .GroupBy(c => c.router)
                .Select(g => (router: g.Key, recipients: g.Select(i => i.recipient).ToList()))
                .ToList();

            var tasks = routerMappings.Select(m => m.router.SendAsync(brokeredMessage.Clone(m.recipients), context, cancellationToken)).ToList();
            return await Task.WhenAll(tasks).PreserveThreadContext();
        }

        private IBrokeredMessage GetResponseMessage(IMessage reply, IBrokeredMessage message, IContext context)
        {
            var builder = this.CreateBrokeredMessageBuilder(context);
            return builder
                .ReplyTo(message)
                .WithContent(reply)
                .BrokeredMessage;
        }

        /// <summary>
        /// Gets the task completion source for the sent message.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <returns>
        /// The task completion source.
        /// </returns>
        private TaskCompletionSource<IMessage> GetTaskCompletionSource(IBrokeredMessage brokeredMessage)
        {
            var taskCompletionSource = new TaskCompletionSource<IMessage>();

            var brokeredMessageId = brokeredMessage.Id;
            var cancellationTokenSource = brokeredMessage.Timeout.HasValue
                                              ? new CancellationTokenSource(brokeredMessage.Timeout.Value)
                                              : null;
            cancellationTokenSource?.Token.Register(
                () =>
                    {
                        cancellationTokenSource.Dispose();

                        if (taskCompletionSource.Task.Status == TaskStatus.WaitingForActivation)
                        {
                            if (this.messageSyncDictionary.TryRemove(brokeredMessageId, out _))
                            {
                                var timeoutException = new TimeoutException(
                                    string.Format(
                                        Strings.MessageBrokerBase_Timeout_Exception,
                                        brokeredMessage.Timeout,
                                        brokeredMessage));
                                this.LogOnTimeout(brokeredMessage, timeoutException);
                                taskCompletionSource.TrySetException(timeoutException);
                            }
                        }
                    });

            var added = this.messageSyncDictionary.TryAdd(brokeredMessageId, (cancellationTokenSource, taskCompletionSource));
            this.LogOnEnqueue(brokeredMessage, added);
            return taskCompletionSource;
        }

        /// <summary>
        /// Logs the message on enqueuing for reply.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="added">True if could be added.</param>
        private void LogOnEnqueue(IBrokeredMessage brokeredMessage, bool added)
        {
            if (!added)
            {
                this.Logger.Error(Strings.MessageBrokerBase_LogOnEnqueue_NotAddedError, brokeredMessage.Id, brokeredMessage.Content, brokeredMessage.Timeout);
                return;
            }

            if (!this.Logger.IsDebugEnabled())
            {
                return;
            }

            this.Logger.Debug(Strings.MessageBrokerBase_LogOnEnqueue_Success, brokeredMessage.Id, brokeredMessage.Content, brokeredMessage.Timeout);
        }

        /// <summary>
        /// Logs the message before send.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        private void LogBeforeSend(IBrokeredMessage brokeredMessage)
        {
            if (!this.Logger.IsDebugEnabled())
            {
                return;
            }

            // TODO localization
            var direction = brokeredMessage.IsOneWay ? "one way" : "with reply";
            this.Logger.Debug($"Sending brokered message (#{brokeredMessage.Id}, {brokeredMessage.Content}) {direction}.");
        }

        /// <summary>
        /// Logs the message on receive.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        private void LogOnReceive(IBrokeredMessage brokeredMessage)
        {
            if (!this.Logger.IsDebugEnabled())
            {
                return;
            }

            // TODO localization
            var reply = brokeredMessage.ReplyToMessageId != null ? $" as reply to {brokeredMessage.ReplyToMessageId}" : string.Empty;
            this.Logger.Debug($"Received brokered message (#{brokeredMessage.Id}, {brokeredMessage.Content}) {reply}.");
        }

        /// <summary>
        /// Logs the message on timeout.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="timeoutException">.</param>
        private void LogOnTimeout(IBrokeredMessage brokeredMessage, TimeoutException timeoutException)
        {
            if (!this.Logger.IsWarningEnabled())
            {
                return;
            }

            // TODO localization
            var reply = brokeredMessage.ReplyToMessageId != null ? $" as reply to {brokeredMessage.ReplyToMessageId}" : string.Empty;
            this.Logger.Warn(timeoutException, $"Timeout after {brokeredMessage.Timeout} for {brokeredMessage} {reply}.");
        }
    }
}