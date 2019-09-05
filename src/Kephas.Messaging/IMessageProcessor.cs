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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    /// <summary>
    /// Application service for processing messages.
    /// </summary>
    /// <remarks>
    /// The message processor is defined as a shared service.
    /// </remarks>
    [SingletonAppServiceContract]
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

    /// <summary>
    /// Extension methods for message processor.
    /// </summary>
    public static class MessageProcessorExtensions
    {
        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <param name="this">The message processor to act on.</param>
        /// <param name="message">The message.</param>
        /// <param name="context">Optional. Context for the message processing.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public static Task<IMessage> ProcessAsync(this IMessageProcessor @this, object message, IMessageProcessingContext context = null, CancellationToken token = default)
        {
            Requires.NotNull(@this, nameof(@this));
            Requires.NotNull(message, nameof(message));

            return @this.ProcessAsync(message as IMessage ?? new MessageAdapter { Message = message }, context, token);
        }
    }
}