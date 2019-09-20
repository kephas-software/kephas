// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDistributedMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDistributedMessageBroker interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for distributed message broker.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IMessageBroker
    {
        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        Task<IMessage> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <param name="context">The publishing context.</param>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        IBrokeredMessageBuilder CreateBrokeredMessageBuilder(IContext context);
    }
}