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

    using Kephas.Logging;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Distributed.Routing.Composition;
    using Kephas.Messaging.Events;
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

        private readonly IContextFactory contextFactory;
        private readonly ICollection<Lazy<IMessageRouter, MessageRouterMetadata>> routerFactories;
        private readonly InitializationMonitor<IMessageBroker> initMonitor;
        private ICollection<(Regex regex, bool isFallback, IMessageRouter router)> routerMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageBroker"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="routerFactories">The router factories.</param>
        public DefaultMessageBroker(
            IContextFactory contextFactory,
            ICollection<Lazy<IMessageRouter, MessageRouterMetadata>> routerFactories)
        {
            this.initMonitor = new InitializationMonitor<IMessageBroker>(this.GetType());
            this.contextFactory = contextFactory;
            this.routerFactories = routerFactories;
        }

        /// <summary>
        /// Dispatches the message asynchronously.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        /// <param name="optionsConfig">Optional. The dispatching options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the response message.
        /// </returns>
        public Task<IMessage> DispatchAsync(
            object message,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            this.initMonitor.AssertIsCompletedSuccessfully();

            using (var context = this.CreateDispatchingContext(message, optionsConfig))
            {
                var brokeredMessage = context.BrokeredMessage;

                if (brokeredMessage.Content == null && string.IsNullOrEmpty(brokeredMessage.ReplyToMessageId))
                {
                    throw new ArgumentNullException(
                        nameof(brokeredMessage),
                        Strings.BrokeredMessage_ContentNullWhenNotReply_Exception
                            .FormatWith(brokeredMessage, nameof(DispatchingContextExtensions.ReplyTo)));
                }

                if (!(brokeredMessage.Recipients?.Any() ?? false) && !(brokeredMessage.Content is IEvent || brokeredMessage.IsOneWay))
                {
                    throw new ArgumentException(
                        Strings.BrokeredMessage_RecipientRequired_Exception.FormatWith(brokeredMessage),
                        nameof(brokeredMessage));
                }

                if (brokeredMessage.IsOneWay)
                {
                    this.LogBeforeSend(brokeredMessage);
                    this.RouterDispatchAsync(brokeredMessage, context, cancellationToken)
                        .ContinueWith(
                            t => this.Logger.Error(string.Format(Strings.DefaultMessageBroker_ErrorsOccurredWhileSending_Exception, brokeredMessage)),
                            TaskContinuationOptions.OnlyOnFaulted);
                    return Task.FromResult((IMessage)null);
                }

                var completionSource = this.GetTaskCompletionSource(brokeredMessage);

                this.LogBeforeSend(brokeredMessage);
                this.RouterDispatchAsync(brokeredMessage, context, cancellationToken)
                    .ContinueWith(
                        t => this.Logger.Error(string.Format(Strings.DefaultMessageBroker_ErrorsOccurredWhileSending_Exception, brokeredMessage)),
                        TaskContinuationOptions.OnlyOnFaulted);

                // Returns an awaiter for the answer, must pair with the original message ID.
                return completionSource.Task;
            }
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
                    regex: string.IsNullOrEmpty(f.Metadata.ReceiverMatch) ? null : new Regex(f.Metadata.ReceiverMatch, RegexOptions.IgnoreCase | RegexOptions.Compiled),
                    isFallback: f.Metadata.IsFallback,
                    asyncRouter: this.TryCreateRouterAsync(f, context, cancellationToken)))
                .ToList();

            await Task.WhenAll(asyncRouterMap.Select(m => m.asyncRouter)).PreserveThreadContext();

            this.routerMap = asyncRouterMap
                                .Where(m => m.asyncRouter.Result != null)
                                .Select(m => (m.regex, m.isFallback, m.asyncRouter.Result))
                                .ToList();
            foreach (var map in this.routerMap)
            {
                map.router.ReplyReceived += this.HandleReplyReceived;
            }

            this.initMonitor.Complete();
        }

        /// <summary>
        /// Creates the dispatching context.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        /// <param name="optionsConfig">Optional. The dispatching options configuration.</param>
        /// <returns>
        /// The new dispatching context.
        /// </returns>
        protected virtual IDispatchingContext CreateDispatchingContext(object message, Action<IDispatchingContext> optionsConfig = null)
        {
            var context = this.contextFactory.CreateContext<DispatchingContext>(message);
            optionsConfig?.Invoke(context);
            return context;
        }

        private async Task<IMessageRouter> TryCreateRouterAsync(Lazy<IMessageRouter, MessageRouterMetadata> f, IContext context, CancellationToken cancellationToken)
        {
            const string InitializationException = nameof(InitializationException);

            if (f.Metadata[InitializationException] is Exception initEx)
            {
                return f.Metadata.IsOptional ? (IMessageRouter)null : throw initEx;
            }

            if (f.IsValueCreated)
            {
                return f.Value;
            }

            try
            {
                var router = f.Value;
                await ServiceHelper.InitializeAsync(router, context, cancellationToken).PreserveThreadContext();
                return router;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, $"Error while trying to create and initialize router '{f.Metadata.AppServiceImplementationType}'.");
                f.Metadata[InitializationException] = ex;
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
        /// Sends the brokered message asynchronously over the physical medium using registered routers.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected virtual async Task RouterDispatchAsync(
            IBrokeredMessage brokeredMessage,
            IDispatchingContext context,
            CancellationToken cancellationToken)
        {
            var results = await this.CollectRouterDispatchResultsAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
            var replies = results
                .Where(t => t.action == RoutingInstruction.Reply)
                .Select(t => t.reply)
                .Distinct() // make from multiple nulls one.
                .ToList();

            if (replies.Count > 0)
            {
                // router results indicating that the broker should process replies
                // will generate cascade router dispatches, even for 'null' replies.
                var replyTasks = replies.Select(r => this.RouterDispatchReplyAsync(r, brokeredMessage, context, cancellationToken));
                await Task.WhenAll(replyTasks).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium using registered routers.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="message">The message to be dispatched.</param>
        /// <param name="sendingContext">Context for the sending.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected virtual async Task RouterDispatchReplyAsync(
            IMessage reply,
            IBrokeredMessage message,
            IDispatchingContext sendingContext,
            CancellationToken cancellationToken)
        {
            using (var replyContext = this.CreateDispatchingContext(
                reply,
                ctx => ctx.Impersonate(sendingContext).ReplyTo(message)))
            {
                await this.RouterDispatchAsync(replyContext.BrokeredMessage, replyContext, cancellationToken).PreserveThreadContext();
            }
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
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the action, reply, and router.
        /// </returns>
        private async Task<ICollection<(RoutingInstruction action, IMessage reply, IMessageRouter router)>> CollectRouterDispatchResultsAsync(
            IBrokeredMessage brokeredMessage,
            IDispatchingContext context,
            CancellationToken cancellationToken)
        {
            if (brokeredMessage.Recipients == null || !brokeredMessage.Recipients.Any())
            {
                var router = this.routerMap.FirstOrDefault(f => f.isFallback).router;
                if (router != null)
                {
                    if (this.Logger.IsDebugEnabled())
                    {
                        this.Logger.Debug($"Message {brokeredMessage} does not have any recipients; using fallback router {router.GetType()}.");
                    }

                    var (action, reply) = await router.DispatchAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();

                    return new[] { (action, reply, router) };
                }

                throw new MessagingException(Strings.DefaultMessageBroker_CannotHandleMessagesWithoutRecipients_Exception);
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

            // optimization for the typical case when there is only one router to handle the recipients.
            if (recipientMappings.Count == 1)
            {
                var router = recipientMappings[0].router;
                if (this.Logger.IsDebugEnabled())
                {
                    this.Logger.Debug($"Message {brokeredMessage} has recipient {recipientMappings[0].recipient}; using router {router.GetType()}.");
                }

                var (action, reply) = await router.DispatchAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
                return new[] { (action, reply, router) };
            }

            // general case when there are more routers for the recipients
            var routerMappings = recipientMappings
                .GroupBy(c => c.router)
                .Select(g => (router: g.Key, recipients: g.Select(i => i.recipient).ToList()))
                .ToList();

            if (this.Logger.IsDebugEnabled())
            {
                foreach (var routerMapping in routerMappings)
                {
                    this.Logger.Debug($"Message {brokeredMessage} has recipients: {string.Join(", ", routerMapping.recipients)}; using router {routerMapping.router.GetType()}.");
                }
            }

            var routerTasks = routerMappings.Select(m => (m.router, task: m.router.DispatchAsync(brokeredMessage.Clone(m.recipients), context, cancellationToken))).ToList();
            await Task.WhenAll(routerTasks.Select(rt => rt.task)).PreserveThreadContext();
            return routerTasks.Select(rt => (rt.task.Result.action, rt.task.Result.reply, rt.router)).ToArray();
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