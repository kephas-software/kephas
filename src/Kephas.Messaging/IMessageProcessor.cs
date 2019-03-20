// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application service for processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for processing messages.
    /// </summary>
    /// <remarks>
    /// The message processor is defined as a shared service.
    /// </remarks>
    [SharedAppServiceContract]
    public interface IMessageProcessor
    {
        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">Context for the message processing.</param>
        /// <param name="token">  The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context = null, CancellationToken token = default);
    }
}