// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDistributedMessageBroker interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for distributed message broker.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IMessageBroker
    {
        /// <summary>
        /// Dispatches the message asynchronously.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        /// <param name="optionsConfig">Optional. The dispatching options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the response message.
        /// </returns>
        Task<IMessage> DispatchAsync(
            object message,
            Action<IDispatchingContext> optionsConfig = null,
            CancellationToken cancellationToken = default);
    }
}