// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message handler provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Interaction;

namespace Kephas.Messaging.HandlerProviders
{
    using System;

    using Kephas.Messaging;
    using Kephas.Services;

    /// <summary>
    /// A default message handler provider.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class DefaultMessageHandlerResolveBehavior : IMessageHandlerResolveBehavior
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
            return true;
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
            IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>? handlers;
            try
            {
                handlers = next() as IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                           ?? Array.Empty<IExportFactory<IMessageHandler, MessageHandlerMetadata>>();
                return handlers.Take(1);
            }
            catch (InterruptSignal interruptSignal)
            {
                handlers = interruptSignal.Result as IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>>;
            }

            return handlers ?? Array.Empty<IExportFactory<IMessageHandler, MessageHandlerMetadata>>();
        }
    }
}