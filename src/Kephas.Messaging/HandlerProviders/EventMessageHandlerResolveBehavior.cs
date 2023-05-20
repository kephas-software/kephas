// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event message handler provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Interaction;

namespace Kephas.Messaging.HandlerProviders
{
    using System;

    using Kephas.Messaging;
    using Kephas.Messaging.Events;
    using Kephas.Services;

    /// <summary>
    /// Strategy service for selecting message handlers for events.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class EventMessageHandlerResolveBehavior : IMessageHandlerResolveBehavior
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
            return typeof(IEvent).IsAssignableFrom(context.EnvelopeType)
                || typeof(IEvent).IsAssignableFrom(context.MessageType);
        }
    }
}