// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerProviderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message handler provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Messaging.HandlerProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging;
    using Kephas.Messaging.Composition;
    using Kephas.Services;

    /// <summary>
    /// Base class for message handler providers.
    /// </summary>
    public abstract class MessageHandlerProviderBase : IMessageHandlerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerProviderBase"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        protected MessageHandlerProviderBase(IMessageMatchService messageMatchService)
        {
            Requires.NotNull(messageMatchService, nameof(messageMatchService));

            this.MessageMatchService = messageMatchService;
        }

        /// <summary>
        /// Gets the message match service.
        /// </summary>
        /// <value>
        /// The message match service.
        /// </value>
        protected IMessageMatchService MessageMatchService { get; }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="envelopeType">The type of the envelope. This is typically the adapter type, if the message does not implement <see cref="IMessage"/>.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public abstract bool CanHandle(Type envelopeType, Type messageType, object messageId);

        /// <summary>
        /// Gets a factory which retrieves the components handling messages of the given type.
        /// </summary>
        /// <param name="handlerFactories">The handler factories.</param>
        /// <param name="envelopeType">The type of the envelope. This is typically the adapter type, if
        ///                            the message does not implement <see cref="IMessage"/>.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// A factory of an enumeration of message handlers.
        /// </returns>
        public virtual Func<IEnumerable<IMessageHandler>> GetHandlersFactory(
            IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories,
            Type envelopeType,
            Type messageType,
            object messageId)
        {
            var orderedHandlers = this.GetOrderedMessageHandlerFactories(handlerFactories, envelopeType, messageType, messageId);
            return () => orderedHandlers.Select(f => f.CreateExportedValue()).ToList();
        }

        /// <summary>
        /// Gets the ordered message handler factories.
        /// </summary>
        /// <param name="handlerFactories">The handler factories.</param>
        /// <param name="envelopeType">The type of the envelope. This is typically the adapter type, if
        ///                            the message does not implement <see cref="IMessage"/>.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// The ordered message handler factories.
        /// </returns>
        protected virtual IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
            GetOrderedMessageHandlerFactories(
                IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories,
                Type envelopeType,
                Type messageType,
                object messageId)
        {
            var orderedFactories = handlerFactories
                .Where(f => this.MessageMatchService.IsMatch(f.Metadata.MessageMatch, envelopeType, messageType, messageId))
                .Order()
                .ToList();
            return orderedFactories;
        }
    }
}