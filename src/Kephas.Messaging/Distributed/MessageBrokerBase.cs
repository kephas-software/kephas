// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBrokerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message broker base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;

    /// <summary>
    /// Base implementation of a <see cref="IMessageBroker"/>.
    /// </summary>
    public abstract class MessageBrokerBase : IMessageBroker
    {
        /// <summary>
        /// The dictionary for message synchronization.
        /// </summary>
        private readonly
            ConcurrentDictionary<string, (CancellationTokenSource cancellationTokenSource,
                TaskCompletionSource<IMessage> taskCompletionSource)> messageSyncDictionary =
                new ConcurrentDictionary<string, (CancellationTokenSource, TaskCompletionSource<IMessage>)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBrokerBase"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        protected MessageBrokerBase(IAppManifest appManifest)
        {
            Requires.NotNull(appManifest, nameof(appManifest));

            this.AppManifest = appManifest;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<IMessageBroker> Logger { get; set; }

        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        /// <value>
        /// The application manifest.
        /// </value>
        public IAppManifest AppManifest { get; }

        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        public virtual Task<IMessage> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));

            if (brokeredMessage.IsOneWay)
            {
                this.LogBeforeSend(brokeredMessage);
                this.SendAsync(brokeredMessage, cancellationToken);
                return Task.FromResult((IMessage)null);
            }

            var taskCompletionSource = this.GetTaskCompletionSource(brokeredMessage);

            this.LogBeforeSend(brokeredMessage);
            this.SendAsync(brokeredMessage, cancellationToken);

            // Returns an awaiter for the answer, must pair with the original message ID.
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Notification method for a received reply.
        /// </summary>
        /// <param name="replyMessage">Message describing the reply.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual Task ReplyReceivedAsync(
            IBrokeredMessage replyMessage,
            CancellationToken cancellationToken = default)
        {
            var replyToMessageId = replyMessage.ReplyToMessageId;
            if (string.IsNullOrEmpty(replyToMessageId))
            {
                this.Logger.Warn(Strings.MessageBrokerBase_MissingReplyToMessageId_Exception, nameof(IBrokeredMessage.ReplyToMessageId), replyMessage.Content);
                return Task.FromResult(0);
            }

            if (!this.messageSyncDictionary.TryRemove(replyToMessageId, out var syncEntry))
            {
                this.Logger.Warn(Strings.MessageBrokerBase_ReplyToMessageNotFound_Exception, replyToMessageId, replyMessage.Content);
                return Task.FromResult(0);
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

            return Task.FromResult(0);
        }

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> CreateBrokeredMessageBuilder<TMessage>()
            where TMessage : BrokeredMessage, new()
        {
            return new BrokeredMessageBuilder<TMessage>(this.AppManifest);
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        protected abstract Task SendAsync(
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken);

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
            var cancellationTokenSource = brokeredMessage.Timeout.HasValue
                                              ? new CancellationTokenSource(brokeredMessage.Timeout.Value)
                                              : null;

            var brokeredMessageId = brokeredMessage.Id;
            cancellationTokenSource?.Token.Register(
                () =>
                    {
                        cancellationTokenSource.Dispose();

                        if (taskCompletionSource.Task.Status == TaskStatus.WaitingForActivation)
                        {
                            if (this.messageSyncDictionary.TryRemove(brokeredMessageId, out var _))
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
                // TODO localization
                this.Logger.Error($"Could not enqueue brokered message (#{brokeredMessage.Id}, {brokeredMessage.Content}) timeout: {brokeredMessage.Timeout}.");
                return;
            }

            if (!this.Logger.IsDebugEnabled())
            {
                return;
            }

            // TODO localization
            this.Logger.Debug($"Enqueue brokered message (#{brokeredMessage.Id}, {brokeredMessage.Content}) timeout: {brokeredMessage.Timeout}.");
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
            if (!this.Logger.IsDebugEnabled())
            {
                return;
            }

            // TODO localization
            var reply = brokeredMessage.ReplyToMessageId != null ? $" as reply to {brokeredMessage.ReplyToMessageId}" : string.Empty;
            this.Logger.Debug(timeoutException, $"Timeout brokered message (#{brokeredMessage.Id}, {brokeredMessage.Content}) {reply}.");
        }
    }
}