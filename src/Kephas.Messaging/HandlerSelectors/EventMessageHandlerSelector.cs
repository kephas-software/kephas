// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventMessageHandlerSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event message handler selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Events;
    using Kephas.Services;

    /// <summary>
    /// Strategy service for selecting message handlers for events.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class EventMessageHandlerSelector : MessageHandlerSelectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessageHandlerSelector"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerFactories">The message handler factories.</param>
        public EventMessageHandlerSelector(
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
            : base(messageMatchService, handlerFactories)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="envelopeType">The type of the envelope. This is typically the adapter type, if the message does not implement <see cref="IMessage"/>.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(Type envelopeType, Type messageType, object messageId)
        {
            return typeof(IEvent).IsAssignableFrom(messageType)
                || typeof(IEvent).IsAssignableFrom(envelopeType);
        }
    }
}