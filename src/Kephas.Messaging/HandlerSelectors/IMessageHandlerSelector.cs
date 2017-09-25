// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandlerSelector.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IMessageHandlerSelector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for message handler selector.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true)]
    public interface IMessageHandlerSelector
    {
        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <remarks>
        /// This is the method by which the selectors are requested to indicate 
        /// whether they are in charge of providing the handlers 
        /// for a specific message type and name.
        /// </remarks>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageName">The message name.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        bool CanHandle(Type messageType, string messageName);

        /// <summary>
        /// Gets a factory which retrieves the components handling messages of the given type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageName">The message name.</param>
        /// <returns>
        /// A factory of an enumeration of message handlers.
        /// </returns>
        Func<IEnumerable<IMessageHandler>> GetHandlersFactory(Type messageType, string messageName);
    }
}