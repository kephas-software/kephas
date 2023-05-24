// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandlerSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Messaging
{
    /// <summary>
    /// Service contract for resolving message handlers.
    /// </summary>
    [AppServiceContract]
    public interface IMessageHandlerResolver
    {
        /// <summary>
        /// Resolves the message handlers for the provided messaging context.
        /// </summary>
        /// <param name="context">The messaging context.</param>
        /// <returns>The matching message handlers.</returns>
        IEnumerable<IMessageHandler<TMessage, TResult>> Resolve<TMessage, TResult>(IMessagingContext context)
            where TMessage : IMessage<TResult>;
    }
}