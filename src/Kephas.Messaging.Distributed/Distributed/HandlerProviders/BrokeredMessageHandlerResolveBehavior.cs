// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message handler provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Interaction;

namespace Kephas.Messaging.Distributed.HandlerProviders
{
    using System;

    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.HandlerProviders;
    using Kephas.Services;

    /// <summary>
    /// A brokered message handler provider.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class BrokeredMessageHandlerResolveBehavior : IMessageHandlerResolveBehavior
    {
        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public bool CanHandle(IMessagingContext context)
        {
            return typeof(IBrokeredMessage).IsAssignableFrom(context.MessageType);
        }

        /// <summary>
        /// Invokes the behavior.
        /// </summary>
        /// <param name="next">The pipeline continuation delegate.</param>
        /// <param name="target">The target.</param>
        /// <param name="args">The operation arguments.</param>
        /// <param name="context">The operation context.</param>
        /// <returns>The invocation result.</returns>
        public object? Invoke(Func<object?> next, IMessageHandlerResolver target, IMessagingContext args, IContext context)
        {
            var handlers = next() as IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>;
            if (CanHandle(args))
            {
                throw new InterruptSignal(handlers?.Take(1));
            }

            return handlers;
        }
    }
}