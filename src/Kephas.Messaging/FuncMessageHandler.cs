// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuncMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the function message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging;

using System;
using System.Threading;
using System.Threading.Tasks;

using Kephas.Services.AttributedModel;

/// <summary>
/// A function message handler.
/// </summary>
/// <typeparam name="TMessage">Type of the message.</typeparam>
[ExcludeFromServices]
public class FuncMessageHandler<TMessage> : IMessageHandler<TMessage>
    where TMessage : class
{
    private readonly Func<TMessage, IMessagingContext, CancellationToken, Task<object?>> handlerFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMessageHandler{TMessage}"/> class.
    /// </summary>
    /// <param name="handlerFunction">The handler function.</param>
    public FuncMessageHandler(Func<TMessage, IMessagingContext, CancellationToken, Task<object?>> handlerFunction)
    {
        handlerFunction = handlerFunction ?? throw new ArgumentNullException(nameof(handlerFunction));

        this.handlerFunction = handlerFunction;
    }

    /// <summary>
    /// Processes the provided message asynchronously and returns a response promise.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <param name="context">The processing context.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// The response promise.
    /// </returns>
    public Task<object?> ProcessAsync(TMessage message, IMessagingContext context, CancellationToken token)
    {
        return this.handlerFunction(message, context, token);
    }
}