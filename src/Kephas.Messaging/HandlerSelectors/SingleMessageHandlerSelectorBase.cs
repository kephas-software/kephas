// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleMessageHandlerSelectorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the single message handler selector base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerSelectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Resources;

    /// <summary>
    /// Base class for message handler selectors requiring a single handler per message type.
    /// </summary>
    public abstract class SingleMessageHandlerSelectorBase : MessageHandlerSelectorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleMessageHandlerSelectorBase"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerFactories">The message handler factories.</param>
        protected SingleMessageHandlerSelectorBase(
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
            : base(messageMatchService, handlerFactories)
        {
        }

        /// <summary>
        /// Gets a factory which retrieves the components handling messages of the given type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// A factory of an enumeration of message handlers.
        /// </returns>
        public override Func<IEnumerable<IMessageHandler>> GetHandlersFactory(Type messageType, object messageId)
        {
            var orderedFactories = this.GetOrderedMessageHandlerFactories(messageType, messageId);

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
                            string.Join(", ", orderedFactories.Select(f => f.Metadata.AppServiceImplementationType?.FullName))));
                }
            }

            var handlerFactory = orderedFactories[0];
            IEnumerable<IMessageHandler> Factory() => new[] { handlerFactory.CreateExportedValue() };
            return Factory;
        }
    }
}