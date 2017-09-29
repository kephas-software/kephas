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
        /// Creates a brokered message builder.
        /// </summary>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        public BrokeredMessageBuilder CreateBrokeredMessageBuilder()
        {
            return new BrokeredMessageBuilder(this.AppManifest);
        }
    }
}