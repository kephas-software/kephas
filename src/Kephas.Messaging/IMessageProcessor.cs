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
using Kephas.Services;
using Kephas.Messaging.Messages;
using Kephas.Reflection;
using Kephas.Threading.Tasks;

namespace Kephas.Messaging;

/// <summary>
/// Application service for processing messages.
/// </summary>
/// <remarks>
/// The message processor is defined as a shared service.
/// </remarks>
[AppServiceContract]
public interface IMessageProcessor
{
    private static readonly MethodInfo ProcessAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IMessageProcessor)null!).ProcessAsync<IMessage<object>, object?>(null!, null!, default));
        
    /// <summary>
    /// Processes the specified message asynchronously and returns the response.
    /// </summary>
    /// <param name="message">
    /// The message to process. If the message does not implement <see cref="IMessage{TResponse}"/>,
    /// an <see cref="IMessageEnvelope{T}"/> instance if created wrapping the message.
    /// </param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <param name="token">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the response message.
    /// </returns>
    async Task<object?> ProcessAsync(object message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
    {
        _ = message ?? throw new ArgumentNullException(nameof(message));

        var typedMessage = message.ToMessage();
        var messageType = typedMessage.GetType();
        var responseType = messageType.GetBaseConstructedGenericOf(typeof(IMessage<>))!;

        var processAsync = ProcessAsyncMethod.MakeGenericMethod(messageType, responseType);
        var task = processAsync.Call<Task>(this, typedMessage, optionsConfig, token);
        await task.PreserveThreadContext();
        return task.GetResult();
    }

    /// <summary>
    /// Processes the specified message asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="message">The message to process.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <param name="token">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the response message.
    /// </returns>
    Task<TResponse> ProcessAsync<TResponse>(IMessage<TResponse> message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
    {
        _ = message ?? throw new ArgumentNullException(nameof(message));

        var messageType = message.GetType();
        var responseType = typeof(TResponse);

        var processAsync = ProcessAsyncMethod.MakeGenericMethod(messageType, responseType);
        return processAsync.Call<Task<TResponse>>(this, message, optionsConfig, token);
    }

    /// <summary>
    /// Processes the specified message asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message to process.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <param name="token">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the response message.
    /// </returns>
    Task ProcessAsync<TMessage>(TMessage message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
        where TMessage : IActionMessage
        => ProcessAsync<object?>((IMessage<object?>)message, optionsConfig, token);

    /// <summary>
    /// Processes the specified message asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message to process.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <param name="token">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the response message.
    /// </returns>
    Task<TResponse> ProcessAsync<TMessage, TResponse>(TMessage message, Action<IMessagingContext>? optionsConfig = null, CancellationToken token = default)
        where TMessage : IMessage<TResponse>;
}