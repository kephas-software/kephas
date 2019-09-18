// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message handler registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.HandlerSelectors;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// A default message handler registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageHandlerRegistry : IMessageHandlerRegistry
    {
        private readonly IList<IMessageHandlerSelector> handlerSelectors;
        private readonly ConcurrentDictionary<string, (Type messageType, object messageId, Func<IEnumerable<IMessageHandler>> factory)> handlerFactories
            = new ConcurrentDictionary<string, (Type messageType, object messageId, Func<IEnumerable<IMessageHandler>> factory)>();

        private readonly ConcurrentBag<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerRegistry;

        private readonly IMessageMatchService messageMatchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageHandlerRegistry"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerSelectorFactories">The handler selector factories.</param>
        /// <param name="handlerFactories">The handler factories.</param>
        public DefaultMessageHandlerRegistry(
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>> handlerSelectorFactories,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
        {
            Requires.NotNull(handlerSelectorFactories, nameof(handlerSelectorFactories));
            Requires.NotNull(handlerFactories, nameof(handlerFactories));

            this.handlerSelectors = handlerSelectorFactories
                .Order()
                .Select(f => f.CreateExportedValue())
                .ToList();
            this.messageMatchService = messageMatchService;
            this.handlerRegistry = new ConcurrentBag<IExportFactory<IMessageHandler, MessageHandlerMetadata>>(handlerFactories);
        }

        /// <summary>
        /// Registers the message handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="metadata">The handler metadata.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        public IMessageHandlerRegistry RegisterHandler(IMessageHandler handler, MessageHandlerMetadata metadata)
        {
            Requires.NotNull(handler, nameof(handler));
            Requires.NotNull(metadata, nameof(metadata));

            this.handlerRegistry.Add(new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler, metadata));
            this.ResetFactoryCache(metadata);

            return this;
        }

        /// <summary>
        /// Resolves the message handlers for the provided message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message handlers.</returns>
        public virtual IEnumerable<IMessageHandler> ResolveMessageHandlers(IMessage message)
        {
            var envelopeType = message.GetType();
            var messageType = this.messageMatchService.GetMessageType(message);
            var messageId = this.messageMatchService.GetMessageId(message);
            var (_, _, messageHandlersFactory) = this.handlerFactories.GetOrAdd($"{envelopeType}/{messageType.FullName}/{messageId}", _ =>
            {
                var handlerSelector = this.handlerSelectors.FirstOrDefault(s => s.CanHandle(envelopeType, messageType, messageId));
                if (handlerSelector == null)
                {
                    return (messageType, messageId, () => null);
                }

                return (messageType, messageId, handlerSelector.GetHandlersFactory(this.handlerRegistry, envelopeType, messageType, messageId));
            });

            var handlers = messageHandlersFactory();
            return handlers ?? new IMessageHandler[0];
        }

        private void ResetFactoryCache(MessageHandlerMetadata metadata)
        {
            IEnumerable<KeyValuePair<string, (Type messageType, object messageId, Func<IEnumerable<IMessageHandler>> factory)>> factoriesToDelete = this.handlerFactories;
            if (metadata.MessageIdMatching == MessageIdMatching.Id)
            {
                factoriesToDelete = factoriesToDelete.Where(t => t.Value.messageId == metadata.MessageId);
            }

            if (metadata.MessageType == null)
            {

            }
            else if (metadata.MessageTypeMatching == MessageTypeMatching.Type)
            {
                factoriesToDelete = factoriesToDelete.Where(t => t.Value.messageType == metadata.MessageType);
            }
            else if (metadata.MessageTypeMatching == MessageTypeMatching.TypeOrHierarchy)
            {
                factoriesToDelete = factoriesToDelete.Where(t => metadata.MessageType.IsAssignableFrom(t.Value.messageType));
            }

            foreach (var factoryToDelete in factoriesToDelete.ToList())
            {
                this.handlerFactories.TryRemove(factoryToDelete.Key, out _);
            }
        }
    }
}
