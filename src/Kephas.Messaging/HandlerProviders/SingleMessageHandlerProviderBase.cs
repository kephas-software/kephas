// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleMessageHandlerProviderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the single message handler provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Messaging.HandlerProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Messaging;
    using Kephas.Messaging.Resources;

    /// <summary>
    /// Base class for message handler providers requiring a single handler per message type.
    /// </summary>
    public abstract class SingleMessageHandlerProviderBase : MessageHandlerProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleMessageHandlerProviderBase"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        protected SingleMessageHandlerProviderBase(IMessageMatchService messageMatchService)
            : base(messageMatchService)
        {
        }

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
        public override Func<IEnumerable<IMessageHandler>> GetHandlersFactory(
            IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories,
            Type envelopeType,
            Type messageType,
            object messageId)
        {
            var orderedFactories = GetOrderedMessageHandlerFactories(handlerFactories, envelopeType, messageType, messageId);

            if (orderedFactories.Count == 0)
            {
                throw new MissingHandlerException(
                    string.Format(
                        Strings.DefaultMessageProcessor_MissingHandler_Exception,
                        $"'{messageType.FullName}/{messageId}'"));
            }

            if (orderedFactories.Count > 1)
            {
                if (orderedFactories[0].Metadata.OverridePriority == orderedFactories[1].Metadata.OverridePriority)
                {
                    throw new AmbiguousMatchException(
                        string.Format(
                            Strings.DefaultMessageProcessor_AmbiguousHandler_Exception,
                            $"'{messageType.FullName}/{messageId}'",
                            string.Join(", ", orderedFactories.Select(f => f.Metadata.ServiceInstanceType?.FullName))));
                }
            }

            var handlerFactory = orderedFactories[0];
            IEnumerable<IMessageHandler> Factory() => new[] { handlerFactory.CreateExportedValue() };
            return Factory;
        }
    }
}