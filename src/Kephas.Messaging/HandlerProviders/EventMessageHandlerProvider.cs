// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event message handler provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    public class EventMessageHandlerProvider : MessageHandlerProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessageHandlerProvider"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        public EventMessageHandlerProvider(
            IMessageMatchService messageMatchService)
            : base(messageMatchService)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(IMessagingContext context)
        {
            return typeof(IEvent).IsAssignableFrom(messageType)
                || typeof(IEvent).IsAssignableFrom(context);
        }
    }
}