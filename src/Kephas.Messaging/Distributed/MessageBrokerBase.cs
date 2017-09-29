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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Base implementation of a <see cref="IMessageBroker"/>.
    /// </summary>
    public abstract class MessageBrokerBase : IMessageBroker
    {
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
        public abstract Task<IMessage> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Notification method for a received reply.
        /// </summary>
        /// <param name="replyMessage">Message describing the reply.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public abstract Task ReplyReceivedAsync(
            IBrokeredMessage replyMessage,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public BrokeredMessageBuilder CreateBrokeredMessageBuilder()
        {
            return new BrokeredMessageBuilder(this.AppManifest);
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}