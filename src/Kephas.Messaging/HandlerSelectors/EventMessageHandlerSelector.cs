// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventMessageHandlerSelector.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the event message handler selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Reflection;

    using Kephas.Composition;
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
        /// <param name="compositionContext">Context for the composition.</param>
        public EventMessageHandlerSelector(ICompositionContext compositionContext)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageName">The message name.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(Type messageType, string messageName)
        {
            return typeof(IEvent).GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo());
        }
    }
}