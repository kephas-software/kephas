// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerSelector.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message handler selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Messaging.Composition;
    using Kephas.Services;

    /// <summary>
    /// A default message handler selector.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class DefaultMessageHandlerSelector : SingleMessageHandlerSelectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageHandlerSelector"/> class.
        /// </summary>
        /// <param name="handlerFactories">The message handler factories.</param>
        public DefaultMessageHandlerSelector(IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
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
            return true;
        }
    }
}