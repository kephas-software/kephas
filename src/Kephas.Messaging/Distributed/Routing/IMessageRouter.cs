// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageRouter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageRouter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for message router.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(MessageRouterAttribute) })]
    public interface IMessageRouter
    {
        /// <summary>
        /// Occurs when a reply for is received to match a request sent from the container message broker.
        /// </summary>
        event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

        /// <summary>
        /// Dispatches the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken);
    }
}
