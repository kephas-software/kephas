// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageHandlerSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message handler selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.HandlerSelectors
{
    using System;

    using Kephas.Messaging.HandlerSelectors;
    using Kephas.Services;

    /// <summary>
    /// A brokered message handler selector.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class BrokeredMessageHandlerSelector : SingleMessageHandlerSelectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageHandlerSelector"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        public BrokeredMessageHandlerSelector(IMessageMatchService messageMatchService)
            : base(messageMatchService)
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
            return typeof(IBrokeredMessage).IsAssignableFrom(messageType);
        }
    }
}