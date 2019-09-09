// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBusMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service bus message router class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Azure.ServiceBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Distributed;
    using Kephas.Services;

    /// <summary>
    /// A service bus message router.
    /// </summary>
    public class ServiceBusMessageRouter : IMessageRouter, IAsyncInitializable
    {
        /// <summary>
        /// Occurs when a reply is received.
        /// </summary>
        public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            // TODO read configuration for the connection.
            throw new NotImplementedException();
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
        public Task SendAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}