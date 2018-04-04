// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageHandlerSelector.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message handler selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.HandlerSelectors
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Messaging.Composition;
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
        /// <param name="handlerFactories">The message handler factories.</param>
        public BrokeredMessageHandlerSelector(IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
            : base(handlerFactories)
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
            return typeof(IBrokeredMessage).GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo());
        }
    }
}