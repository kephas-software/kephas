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

    using Kephas.Application;
    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a <see cref="IMessageBroker"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageBroker : Loggable, IMessageBroker, IAsyncInitializable, IAsyncFinalizable, IIdentifiable
    {
        /// <summary>
        /// The dictionary for message synchronization.
        /// </summary>
        private readonly
            ConcurrentDictionary<string, (CancellationTokenSource? cancellationTokenSource,
                TaskCompletionSource<IMessage?> taskCompletionSource)> messageSyncDictionary = new ();

        private readonly IInjectableFactory injectableFactory;
        private readonly IOrderedLazyServiceCollection<IMessageRouter, MessageRouterMetadata> routerFactories;
        private readonly InitializationMonitor<IMessageBroker> initMonitor;
        private ICollection<RouterEntry>? routerMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageBroker"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="routerFactories">The enabled router factories.</param>
        public DefaultMessageBroker(
            IInjectableFactory injectableFactory,
            IAppRuntime appRuntime,
            IEnabledLazyServiceCollection<IMessageRouter, MessageRouterMetadata> routerFactories)
            : base(injectableFactory)
        {
            this.initMonitor = new InitializationMonitor<IMessageBroker>(this.GetType());
            this.injectableFactory = injectableFactory;
            this.routerFactories = routerFactories.Order();
            this.Id = $"{appRuntime.GetAppId()}/{appRuntime.GetAppInstanceId()}";
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; }

        /// <summary>
        /// Dispatches the message asynchronously.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        /// <param name="optionsConfig">Optional. The dispatching options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the response message.
        /// </returns>
        public Task<IMessage?> DispatchAsync(
            object message,
            Action<IDispatchingContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            this.initMonitor.AssertIsCompletedSuccessfully();

            using var context = this.CreateDispatchingContext(message, optionsConfig);

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

            if ((brokeredMessage.Recipients?.Count() ?? 2) > 1 && !brokeredMessage.IsOneWay)
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
                        t => this.Logger.Error(t.Exception, Strings.DefaultMessageBroker_ErrorsOccurredWhileSending_Exception, brokeredMessage),
                        TaskContinuationOptions.OnlyOnFaulted);
                return Task.FromResult<IMessage?>(null);
            }

            var completionSource = this.GetTaskCompletionSource(brokeredMessage);

            this.LogBeforeSend(brokeredMessage);
            this.RouterDispatchAsync(brokeredMessage, context, cancellationToken)
                .ContinueWith(
                    t => this.Logger.Error(t.Exception, Strings.DefaultMessageBroker_ErrorsOccurredWhileSending_Exception, brokeredMessage),
                    TaskContinuationOptions.OnlyOnFaulted);

            // Returns an awaiter for the answer, must pair with the reply ID.
            return completionSource.Task;
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public virtual async Task InitializeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.initMonitor.Start();

            var asyncRouterMap = this.routerFactories
                .Select(f => (
                    regex: string.IsNullOrEmpty(f.Metadata.ReceiverMatch)
                        ? f.Metadata.ReceiverMatchProviderType == null
                            ? null
                            : this.GetReceiverMatch(f.Metadata.ReceiverMatchProviderType, context)
                        : this.GetReceiverMatch(f.Metadata.ReceiverMatch, context),
                    isFallback: f.Metadata.IsFallback,
                    asyncRouter: this.TryCreateRouterAsync(f, context, cancellationToken),
                    metadata: f.Metadata))
                .ToList();

            await Task.WhenAll(asyncRouterMap.Select(m => m.asyncRouter)).PreserveThreadContext();

            this.routerMap = asyncRouterMap
                                .Where(m => m.asyncRouter.Result != null)
                                .Select(m => new RouterEntry(m.regex, m.isFallback, m.asyncRouter.Result!, m.metadata))
                                .ToList();
            foreach (var map in this.routerMap)
            {
                map.Router.ReplyReceived += this.HandleReplyReceived;
            }

            this.Logger.Debug("Message broker {messageBroker} registered message routers: {routers}.", this, this.Id, this.routerMap.Select(r => r.Metadata.ServiceType).ToArray());

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
        public virtual async Task FinalizeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            if (this.routerMap != null)
            {
                foreach (var (_, _, router, _) in this.routerMap)
                {
                    router.ReplyReceived -= this.HandleReplyReceived;
                    await ServiceHelper.FinalizeAsync(router, cancellationToken: cancellationToken).PreserveThreadContext();
                }
            }

            this.initMonitor.Reset();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Id}";
        }

        /// <summary>
        /// Creates the dispatching context.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        /// <param name="optionsConfig">Optional. The dispatching options configuration.</param>
        /// <returns>
        /// The new dispatching context.
        /// </returns>
        protected virtual IDispatchingContext CreateDispatchingContext(object message, Action<IDispatchingContext>? optionsConfig = null)
        {
            var context = this.injectableFactory.Create<DispatchingContext>(message).Merge(optionsConfig);
            return context;
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
                var replyTasks = replies.Select(r => this.RouterDispatchReplyAsync(r!, brokeredMessage, context, cancellationToken));
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
            using var replyContext = this.CreateDispatchingContext(
                reply,
                ctx => ctx.Impersonate(sendingContext).ReplyTo(message));
            await this.RouterDispatchAsync(replyContext.BrokeredMessage, replyContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Notification method for receiving a reply.
        /// </summary>
        /// <param name="router">The router which received the message.</param>
        /// <param name="replyMessage">Message describing the reply.</param>
        /// <param name="context">Optional. The reply context.</param>
        protected virtual void ReceiveReply(
            IMessageRouter router,
            IBrokeredMessage replyMessage,
            IContext? context = null)
        {
            var replyToMessageId = replyMessage.ReplyToMessageId;
            if (string.IsNullOrEmpty(replyToMessageId))
            {
                this.Logger.Warn(Strings.DefaultMessageBroker_MissingReplyToMessageId_Exception, nameof(IBrokeredMessage.ReplyToMessageId), replyMessage);
                return;
            }

            if (!this.messageSyncDictionary.TryRemove(replyToMessageId, out var syncEntry))
            {
                // the original request is not found
                // check whether the received reply is handled by a different router than the one
                // that received the message, in which case it might need redirection,
                // for example when the message is for another machine.
                var redirectContext = this.CreateDispatchingContext(replyMessage, ctx => ctx.InputRouter(router));
                this.RouterDispatchAsync(redirectContext.BrokeredMessage, redirectContext, default)
                    .ContinueWith(
                        t =>
                        {
                            if (t.IsFaulted)
                            {
                                this.Logger.Error(Strings.DefaultMessageBroker_ErrorsOccurredWhileRedirecting_Exception, replyMessage);
                            }

                            redirectContext?.Dispose();
                        });

                return;
            }

            this.LogOnReceive(replyMessage);

            syncEntry.cancellationTokenSource?.Dispose();

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
        /// Selects the router handling the message with the provided predicate.
        /// </summary>
        /// <param name="map">The router map.</param>
        /// <param name="predicate">The select predicate.</param>
        /// <returns>The selected message router or <c>null</c>.</returns>
        protected virtual IMessageRouter? SelectRouter(
            ICollection<RouterEntry> map,
            Func<RouterEntry, bool> predicate)
        {
            return map.FirstOrDefault(predicate)?.Router;
        }

        private Regex? GetReceiverMatch(Type receiverMatchProviderType, IContext? context)
        {
            if (!typeof(IReceiverMatchProvider).IsAssignableFrom(receiverMatchProviderType))
            {
                throw new InvalidOperationException(Strings.DefaultMessageBroker_BadReceiverMatchProviderType_Exception.FormatWith(receiverMatchProviderType, typeof(IReceiverMatchProvider)));
            }

            var provider = (IReceiverMatchProvider)this.injectableFactory.Create(receiverMatchProviderType);
            var receiverMatch = provider.GetReceiverMatch(context);
            return this.GetReceiverMatch(receiverMatch, context);
        }

        private Regex? GetReceiverMatch(string receiverMatch, IContext? context)
        {
            return string.IsNullOrEmpty(receiverMatch)
                ? null
                : new Regex(receiverMatch, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private async Task<IMessageRouter?> TryCreateRouterAsync(Lazy<IMessageRouter, MessageRouterMetadata> f, IContext? context, CancellationToken cancellationToken)
        {
            const string InitializationException = nameof(InitializationException);
            if (f.Metadata[InitializationException] is Exception initEx)
            {
                return f.Metadata.IsOptional ? (IMessageRouter?)null : throw initEx;
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
                this.Logger.Warn(ex, "Error while trying to create and initialize router '{router}'.", f.Metadata.ServiceType);
                f.Metadata[InitializationException] = ex;
                if (f.Metadata.IsOptional)
                {
                    this.Logger.Warn("Router '{router}' will be ignored.", f.Metadata.ServiceType);
                    return null;
                }

                throw;
            }
        }

        private void HandleReplyReceived(object sender, ReplyReceivedEventArgs e)
        {
            this.ReceiveReply((IMessageRouter)sender, e.Message, e.Context);
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
        private async Task<ICollection<(RoutingInstruction action, IMessage? reply, IMessageRouter router)>> CollectRouterDispatchResultsAsync(
            IBrokeredMessage brokeredMessage,
            IDispatchingContext context,
            CancellationToken cancellationToken)
        {
            if (brokeredMessage.Recipients == null || !brokeredMessage.Recipients.Any())
            {
                var router = this.SelectRouter(this.routerMap!, f => f.IsFallback);
                if (router == null)
                {
                    throw new MessagingException(Strings.DefaultMessageBroker_CannotHandleMessagesWithoutRecipients_Exception);
                }

                var (action, reply) = await this.GetRouterDispatchResultAsync(brokeredMessage, context, router, cancellationToken).PreserveThreadContext();
                return new[] { (action, reply, router) };
            }

            var recipientMappings = brokeredMessage.Recipients
                .Select(r => (recipient: r, router: this.SelectRouter(this.routerMap!, f => f.IsFallback || (f.Regex?.IsMatch(r.Url.ToString()) ?? false))))
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
                var router = recipientMappings[0].router!;
                var (action, reply) = await this.GetRouterDispatchResultAsync(brokeredMessage, context, router, cancellationToken).PreserveThreadContext();
                return new[] { (action, reply, router) };
            }

            // general case when there are more routers for the recipients
            var routerMappings = recipientMappings
                .GroupBy(c => c.router)
                .Select(g => (router: g.Key, recipients: g.Select(i => i.recipient).ToList()))
                .ToList();

            var routerTasks = routerMappings.Select(m => (m.router, task: this.GetRouterDispatchResultAsync(brokeredMessage.Clone(m.recipients), context, m.router, cancellationToken))).ToList();
            await Task.WhenAll(routerTasks.Select(rt => rt.task)).PreserveThreadContext();
            return routerTasks.Select(rt => (rt.task.Result.action, rt.task.Result.reply, rt.router!)).ToArray();
        }

        private async Task<(RoutingInstruction action, IMessage? reply)> GetRouterDispatchResultAsync(
            IBrokeredMessage brokeredMessage,
            IDispatchingContext context,
            IMessageRouter router,
            CancellationToken cancellationToken)
        {
            if (this.Logger.IsDebugEnabled())
            {
                var recipients = (brokeredMessage.Recipients?.Any() ?? false)
                    ? $"recipients : {string.Join(", ", brokeredMessage.Recipients)}"
                    : $"no recipients";
                this.Logger.Debug("Message {message} has {recipients}; using router {router}.", brokeredMessage, recipients, router.GetType());
            }

            if (context.InputRouter == router && !string.IsNullOrEmpty(brokeredMessage.ReplyToMessageId))
            {
                this.Logger.Warn(Strings.DefaultMessageBroker_ReplyToMessageNotFound_Exception, brokeredMessage);
                return (RoutingInstruction.None, (IMessage?)null);
            }

            return await router.DispatchAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
        }

        private TaskCompletionSource<IMessage?> GetTaskCompletionSource(IBrokeredMessage brokeredMessage)
        {
            var taskCompletionSource = new TaskCompletionSource<IMessage?>();

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
                                        Strings.DefaultMessageBroker_Timeout_Exception,
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
                this.Logger.Error(Strings.DefaultMessageBroker_LogOnEnqueue_NotAddedError, brokeredMessage.Id, brokeredMessage, brokeredMessage.Timeout);
                return;
            }

            if (!this.Logger.IsDebugEnabled())
            {
                return;
            }

            this.Logger.Debug(Strings.DefaultMessageBroker_LogOnEnqueue_Success, brokeredMessage.Id, brokeredMessage, brokeredMessage.Timeout);
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

            this.Logger.Debug("Sending brokered message '{message}'.", brokeredMessage);
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
            this.Logger.Debug("Received brokered message '{message}'.", brokeredMessage);
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
            this.Logger.Warn(timeoutException, "Timeout after {timeout:c} for '{message}'.", brokeredMessage.Timeout, brokeredMessage);
        }

        /// <summary>
        /// Entry for a message router.
        /// </summary>
        /// <param name="Regex">The regular expression used for matching.</param>
        /// <param name="IsFallback">Indicates whether this is a fallback router.</param>
        /// <param name="Router">The router.</param>
        /// <param name="Metadata">The metadata.</param>
        protected record RouterEntry(Regex? Regex, bool IsFallback, IMessageRouter Router, MessageRouterMetadata Metadata);
    }
}