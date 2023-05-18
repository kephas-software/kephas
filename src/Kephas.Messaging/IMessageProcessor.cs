// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application service for processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.Reflection;
using Kephas.Threading.Tasks;

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
    [AppServiceContract]
    public interface IMessageProcessor
    {
        private static readonly MethodInfo ProcessAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IMessageProcessor)null!).ProcessAsync<IMessage<object?>, object?>(null!, null!, default));
        
        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the response message.
        /// </returns>
        async Task<object?> ProcessAsync(object message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));

            var typedMessage = message.ToMessage();
            var messageType = message.GetType();
            var resultType = messageType.GetBaseConstructedGenericOf(typeof(IMessage<>))!;

            var processAsync = ProcessAsyncMethod.MakeGenericMethod(messageType, resultType);
            var task = processAsync.Call<Task>(this, typedMessage, optionsConfig, token);
            await task.PreserveThreadContext();
            return task.GetResult();
        }

        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="message">The message to process.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the response message.
        /// </returns>
        Task<TResult> ProcessAsync<TMessage, TResult>(TMessage message, Action<IMessagingContext<TMessage, TResult>>? optionsConfig = null, CancellationToken token = default)
            where TMessage : IMessage<TResult>;
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
        public static Task<object?> ProcessAsync(this IMessageProcessor @this, object message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            _ = message ?? throw new ArgumentNullException(nameof(message));

            return @this.ProcessAsync(message.ToMessage(), optionsConfig, token);
        }
    }
}