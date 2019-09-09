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
        private ICollection<(Regex regex, bool isFallback, IMessageRouter router)> routerMap;

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
                this.SendAsync(brokeredMessage, context, cancellationToken);
                return Task.FromResult((IMessage)null);
            }

            var taskCompletionSource = this.GetTaskCompletionSource(brokeredMessage);

            this.LogBeforeSend(brokeredMessage);
            this.SendAsync(brokeredMessage, context, cancellationToken);

            // Returns an awaiter for the answer, must pair with the original message ID.
            return taskCompletionSource.Task;
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
                var exception = new MessagingException(exceptionMessage.Exception.Message);
                syncEntry.taskCompletionSource.SetException(exception);
            }
            else
            {
                syncEntry.taskCompletionSource.SetResult(replyMessage.Content);
            }
        }

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <param name="context">Optional. The sending context.</param>
        /// <param name="brokeredMessage">Optional. The brokered message.</param>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public virtual IBrokeredMessageBuilder CreateBrokeredMessageBuilder(
            IContext context,
            IBrokeredMessage brokeredMessage = null)
        {
            var builder = this.builderFactory.CreateExportedValue();
            if (brokeredMessage != null)
            {
                builder.Of(brokeredMessage);
            }

            if (builder is IInitializable initializableBuilder)
            {
                initializableBuilder.Initialize(context);
            }

            return builder;
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
                isFallback: f.Metadata.IsFallback,
                asyncRouter: f.CreateExportedValueAsync(context)))
                .ToList();

            await Task.WhenAll(asyncRouterMap.Select(m => m.asyncRouter)).PreserveThreadContext();

            this.routerMap = asyncRouterMap.Select(m => (m.regex, m.isFallback, m.asyncRouter.Result)).ToList();
            foreach (var map in this.routerMap)
            {
                map.router.ReplyReceived += this.HandleReplyReceived;
            }

            this.initMonitor.Complete();
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            foreach (var map in this.routerMap)
            {
                map.router.ReplyReceived -= this.HandleReplyReceived;
            }

            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Messaging.Distributed.MessageBrokerBase
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        protected virtual Task SendAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken)
        {
            if (brokeredMessage.Recipients == null || !brokeredMessage.Recipients.Any())
            {
                var router = this.routerMap.FirstOrDefault(f => f.isFallback).router;
                if (router != null)
                {
                    return router.SendAsync(brokeredMessage, context, cancellationToken);
                }
                else
                {
                    throw new MessagingException(Strings.DefaultMessageBroker_CannotHandleMessagesWithoutRecipients_Exception);
                }
            }

            var recipientMappings = brokeredMessage.Recipients
                .Select(r => (recipient: r, router: this.routerMap.FirstOrDefault(f => f.isFallback || (f.regex?.IsMatch(r.Url.ToString()) ?? false)).router))
                .ToList();
            var unhandledRecipients = recipientMappings
                .Where(c => c.router == null)
                .Select(m => m.recipient)
                .ToList();
            if (unhandledRecipients.Count > 0)
            {
                throw new MessagingException(string.Format(Strings.DefaultMessageBroker_NoMessageRoutersCanHandleRecipients_Exception, string.Join(", ", unhandledRecipients)));
            }

            var routerMappings = recipientMappings
                .GroupBy(c => c.router)
                .Select(g => (router: g.Key, recipients: g.Select(i => i.recipient).ToList()))
                .ToList();

            var tasks = routerMappings.Select(m => m.router.SendAsync(brokeredMessage.Clone(m.recipients), context, cancellationToken)).ToList();
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Handles the reply received event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event information.</param>
        protected virtual void HandleReplyReceived(object sender, ReplyReceivedEventArgs e)
        {
            this.ReceiveReply(e.Message, e.Context);
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