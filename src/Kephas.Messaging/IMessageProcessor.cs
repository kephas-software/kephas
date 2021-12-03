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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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
        /// <param name="message">The message to process.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the response message.
        /// </returns>
        Task<IMessage?> ProcessAsync(IMessage message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default);
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
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the response message.
        /// </returns>
        public static Task<IMessage?> ProcessAsync(this IMessageProcessor @this, object message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
        {
            @this = @this ?? throw new System.ArgumentNullException(nameof(@this));
            message = message ?? throw new ArgumentNullException(nameof(message));

            return @this.ProcessAsync(message.ToMessage(), optionsConfig, token);
        }
    }
}