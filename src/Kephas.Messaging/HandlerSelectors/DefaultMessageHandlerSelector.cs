// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerFactories">The message handler factories.</param>
        public DefaultMessageHandlerSelector(
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
            : base(messageMatchService, handlerFactories)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(Type messageType, object messageId)
        {
            return true;
        }
    }
}