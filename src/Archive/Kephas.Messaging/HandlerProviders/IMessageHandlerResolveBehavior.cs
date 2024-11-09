// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageHandlerProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Interaction;
using Kephas.Pipelines;

namespace Kephas.Messaging.HandlerProviders
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for message handler provider.
    /// </summary>
    public interface IMessageHandlerResolveBehavior : IPipelineBehavior<IMessageHandlerResolver, IMessagingContext, IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>>
    {
        /// <summary>
        /// Indicates whether the resolve pipeline behavior can handle the indicated message type.
        /// </summary>
        /// <param name="context">The messaging context.</param>
        /// <returns>
        /// True if the resolve pipeline behavior can handle the message type, false if not.
        /// </returns>
        bool CanHandle(IMessagingContext context);

        /// <summary>
        /// Invokes the behavior.
        /// </summary>
        /// <param name="next">The pipeline continuation delegate.</param>
        /// <param name="target">The target.</param>
        /// <param name="args">The operation arguments.</param>
        /// <param name="context">The operation context.</param>
        /// <returns>The invocation result.</returns>
        object? IPipelineBehavior<IMessageHandlerResolver, IMessagingContext, IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>>.Invoke(Func<object?> next, IMessageHandlerResolver target, IMessagingContext args, IContext context)
        {
            var handlers = next();
            if (CanHandle(args))
            {
                throw new InterruptSignal(handlers);
            }

            return handlers;
        }
    }
}