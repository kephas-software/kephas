// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandlerRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageHandlerRegistry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.Messaging.Messages;
using Kephas.Reflection;
using Kephas.Services;
using Kephas.Threading.Tasks;

namespace Kephas.Messaging;

/// <summary>
/// Interface for message handler registry.
/// </summary>
[AppServiceContract]
public interface IMessageHandlerRegistry
{
    private static readonly MethodInfo RegisterFuncHandlerMethod = ReflectionHelper.GetGenericMethodOf(_ =>
        ((IMessageHandlerRegistry)null!).RegisterFuncHandler<IMessage<object?>, object?>(null!));
    
    /// <summary>
    /// Registers the message handler factory.
    /// </summary>
    /// <param name="handlerFactory">The handler factory.</param>
    /// <param name="metadata">The handler metadata.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    public IMessageHandlerRegistry RegisterHandler(
        Func<IServiceProvider, IMessageHandler> handlerFactory,
        MessageHandlerMetadata metadata);

    /// <summary>
    /// Registers the handler.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="handlerFunction">The handler function.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    IMessageHandlerRegistry RegisterHandler<TMessage, TResponse>(
        Func<TMessage, IMessagingContext, CancellationToken, Task<TResponse>> handlerFunction)
        where TMessage : class
    {
        var messageType = typeof(TMessage);
        if (typeof(IMessageBase).IsAssignableFrom(messageType))
        {
            var genericMessageType = messageType.GetBaseConstructedGenericOf(typeof(IMessage<>));
            var responseType = genericMessageType?.GenericTypeArguments[0];
            if (responseType is not null)
            {
                var registerFuncHandler = RegisterFuncHandlerMethod.MakeGenericMethod(messageType, responseType);
                return registerFuncHandler.Call<IMessageHandlerRegistry>(this, handlerFunction);
            }
        }

        return RegisterFuncHandler<IMessageEnvelope<TMessage>, object?>(
            async (envelope, context, token) =>
            {
                var response = await handlerFunction(envelope.GetContent(), context, token).PreserveThreadContext();
                return response;
            });
    }

    /// <summary>
    /// Registers the handler.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="handlerFunction">The synchronous handler function.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    IMessageHandlerRegistry RegisterHandler<TMessage, TResponse>(
        Func<TMessage, IMessagingContext, TResponse> handlerFunction)
        where TMessage : class =>
        RegisterHandler<TMessage, TResponse>((message, context, _) => Task.FromResult(handlerFunction(message, context)));

    /// <summary>
    /// Registers the handler.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="handlerFunction">The synchronous handler function.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    IMessageHandlerRegistry RegisterHandler<TMessage, TResponse>(
        Func<TMessage, TResponse> handlerFunction)
        where TMessage : class =>
        RegisterHandler<TMessage, TResponse>((message, _, _) => Task.FromResult(handlerFunction(message)));

    /// <summary>
    /// Registers the handler.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="handlerFunction">The handler function.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    IMessageHandlerRegistry RegisterHandler<TMessage>(
        Func<TMessage, IMessagingContext, CancellationToken, Task> handlerFunction)
        where TMessage : class =>
        RegisterHandler<TMessage, object?>(async (message, context, token) =>
        {
            await handlerFunction(message, context, token).PreserveThreadContext();
            return Task.FromResult<object?>(null);
        });

    /// <summary>
    /// Registers the handler.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="handlerFunction">The synchronous handler function.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    IMessageHandlerRegistry RegisterHandler<TMessage>(Action<TMessage> handlerFunction)
        where TMessage : class =>
        RegisterHandler<TMessage, object?>((message, _, _) =>
        {
            handlerFunction(message);
            return Task.FromResult<object?>(null);
        });

    /// <summary>
    /// Registers the handler.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="handlerFunction">The synchronous handler function.</param>
    /// <returns>
    /// This message handler registry.
    /// </returns>
    IMessageHandlerRegistry RegisterHandler<TMessage>(Action<TMessage, IMessagingContext> handlerFunction)
        where TMessage : class =>
        RegisterHandler<TMessage, object?>((message, context, _) =>
        {
            handlerFunction(message, context);
            return Task.FromResult<object?>(null);
        });

    /// <summary>
    /// Resolves the message handlers for the provided message.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>The matching message handlers.</returns>
    IEnumerable<IMessageHandler<TMessage, TResult>> ResolveMessageHandlers<TMessage, TResult>(IMessagingContext context)
        where TMessage : IMessage<TResult>;

    private IMessageHandlerRegistry RegisterFuncHandler<TMessage, TResponse>(
        Func<TMessage, IMessagingContext, CancellationToken, Task<TResponse>> handlerFunction)
        where TMessage : IMessage<TResponse>
    {
        return RegisterHandler(
            _ => new FuncMessageHandler<TMessage, TResponse>(handlerFunction),
            new MessageHandlerMetadata(typeof(TMessage)));
    }
}
