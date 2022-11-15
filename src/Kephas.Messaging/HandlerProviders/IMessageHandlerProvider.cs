// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageHandlerProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerProviders
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;
    using Kephas.Services;

    /// <summary>
    /// Interface for message handler provider.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IMessageHandlerProvider
    {
        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <remarks>
        /// This is the method by which the selectors are requested to indicate
        /// whether they are in charge of providing the handlers
        /// for a specific message type and ID.
        /// </remarks>
        /// <param name="envelopeType">The type of the envelope. This is typically the adapter type, if the message does not implement <see cref="IMessage"/>.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        bool CanHandle(Type envelopeType, Type messageType, object messageId);

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
        Func<IEnumerable<IMessageHandler>> GetHandlersFactory(
            IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories,
            Type envelopeType,
            Type messageType,
            object messageId);
    }
}