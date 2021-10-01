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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Messaging.HandlerProviders;
    using Kephas.Services;

    /// <summary>
    /// A default message handler registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageHandlerRegistry : IMessageHandlerRegistry
    {
        private readonly IList<IMessageHandlerProvider> handlerProviders;
        private readonly ConcurrentDictionary<string, (Type envelopeType, Type messageType, object messageId, Func<IEnumerable<IMessageHandler>> factory)> handlerFactories
            = new ConcurrentDictionary<string, (Type envelopeType, Type messageType, object messageId, Func<IEnumerable<IMessageHandler>> factory)>();

        private readonly ConcurrentBag<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerRegistry;

        private readonly IMessageMatchService messageMatchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageHandlerRegistry"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerProviderFactories">The handler provider factories.</param>
        /// <param name="handlerFactories">The handler factories.</param>
        public DefaultMessageHandlerRegistry(
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessageHandlerProvider, AppServiceMetadata>> handlerProviderFactories,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories)
        {
            Requires.NotNull(messageMatchService, nameof(messageMatchService));
            Requires.NotNull(handlerProviderFactories, nameof(handlerProviderFactories));
            Requires.NotNull(handlerFactories, nameof(handlerFactories));

            this.handlerProviders = handlerProviderFactories
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
            this.ResetFactoryCache(metadata.MessageMatch);

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
            var (_, _, _, messageHandlersFactory) = this.handlerFactories.GetOrAdd($"{envelopeType}/{messageType}/{messageId}", _ =>
            {
                var handlerProvider = this.handlerProviders.FirstOrDefault(s => s.CanHandle(envelopeType, messageType, messageId));
                if (handlerProvider == null)
                {
                    return (envelopeType, messageType, messageId, () => null);
                }

                return (envelopeType, messageType, messageId, handlerProvider.GetHandlersFactory(this.handlerRegistry, envelopeType, messageType, messageId));
            });

            var handlers = messageHandlersFactory();
            return handlers ?? Array.Empty<IMessageHandler>();
        }

        private void ResetFactoryCache(IMessageMatch messageMatch)
        {
            // remove all factories which match the metadata match.
            var factoriesToDelete = this.handlerFactories
                .Where(f => this.messageMatchService.IsMatch(messageMatch, f.Value.envelopeType, f.Value.messageType, f.Value.messageId))
                .ToList();

            foreach (var factoryToDelete in factoriesToDelete)
            {
                this.handlerFactories.TryRemove(factoryToDelete.Key, out _);
            }
        }
    }
}
