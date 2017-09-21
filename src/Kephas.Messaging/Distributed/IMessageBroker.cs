// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDistributedMessageBroker.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// Shared application service contract for distributed message broker.
    /// </summary>
    [SharedAppServiceContract]
    public interface IMessageBroker
    {
        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        Task<IMessage> DispatchAsync(IBrokeredMessage brokeredMessage, CancellationToken cancellationToken = default);
    }
}