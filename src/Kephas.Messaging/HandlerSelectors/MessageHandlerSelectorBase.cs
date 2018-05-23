// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerSelectorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message handler selector base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Composition;

    /// <summary>
    /// Base class for message handler selectors.
    /// </summary>
    public abstract class MessageHandlerSelectorBase : IMessageHandlerSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerSelectorBase"/> class.
        /// </summary>
        /// <param name="handlerFactories">The message handler factories.</param>
        protected MessageHandlerSelectorBase(IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
        {
            Requires.NotNull(handlerFactories, nameof(handlerFactories));

            this.HandlerFactories = handlerFactories;
        }

        /// <summary>
        /// Gets the handler factories.
        /// </summary>
        /// <value>
        /// The handler factories.
        /// </value>
        protected IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> HandlerFactories { get; }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public abstract bool CanHandle(Type messageType, object messageId);

        /// <summary>
        /// Gets a factory which retrieves the components handling messages of the given type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// A factory of an enumeration of message handlers.
        /// </returns>
        public virtual Func<IEnumerable<IMessageHandler>> GetHandlersFactory(Type messageType, object messageId)
        {
            var orderedHandlers = this.GetOrderedMessageHandlerFactories(messageType, messageId);
            return () => orderedHandlers.Select(f => f.CreateExportedValue()).ToList();
        }

        /// <summary>
        /// Gets the ordered message handler factories.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// The ordered message handler factories.
        /// </returns>
        protected virtual IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
            GetOrderedMessageHandlerFactories(Type messageType, object messageId)
        {
            var orderedFactories = this.HandlerFactories.Where(
                    f =>
                        {
                            var m = f.Metadata;
                            return ((m.MessageTypeMatching == MessageTypeMatching.Type && m.MessageType == messageType)
                                        || (m.MessageTypeMatching == MessageTypeMatching.TypeOrHierarchy && m.MessageType.IsAssignableFrom(messageType)))
                                   && ((m.MessageIdMatching == MessageIdMatching.Id && Equals(m.MessageId, messageId))
                                        || m.MessageIdMatching == MessageIdMatching.All);
                        })
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ToList();
            return orderedFactories;
        }
    }
}