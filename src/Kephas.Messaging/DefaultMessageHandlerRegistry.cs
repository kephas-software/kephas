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

    using Kephas.Services;
    using Kephas.Messaging.HandlerProviders;
    using Kephas.Services;

    /// <summary>
    /// A default message handler registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageHandlerRegistry : IMessageHandlerRegistry
    {
        private record HandlerEntry(Type EnvelopeType, Type MessageType, object? MessageId, IEnumerable<Func<IMessageHandler>>? Factories);
        
        private readonly IList<IMessageHandlerProvider> handlerProviders;
        private readonly ConcurrentDictionary<string, HandlerEntry> handlerFactories = new();

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
            messageMatchService = messageMatchService ?? throw new System.ArgumentNullException(nameof(messageMatchService));
            handlerProviderFactories = handlerProviderFactories ?? throw new System.ArgumentNullException(nameof(handlerProviderFactories));
            handlerFactories = handlerFactories ?? throw new System.ArgumentNullException(nameof(handlerFactories));

            this.handlerProviders = handlerProviderFactories
                .Order()
                .Select(f => f.CreateExportedValue())
                .ToList();
            this.messageMatchService = messageMatchService;
            this.handlerRegistry = new ConcurrentBag<IExportFactory<IMessageHandler, MessageHandlerMetadata>>(handlerFactories);
        }

        /// <summary>
        /// Registers the message handler factory.
        /// </summary>
        /// <param name="handlerFactory">The handler factory.</param>
        /// <param name="metadata">The handler metadata.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        public IMessageHandlerRegistry RegisterHandler(Func<IServiceProvider, IMessageHandler> handlerFactory, MessageHandlerMetadata metadata)
        {
            handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
            metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));

            this.handlerRegistry.Add(new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handlerFactory(), metadata));
            this.ResetFactoryCache(metadata.MessageMatch);

            return this;
        }

        /// <summary>
        /// Resolves the message handlers for the provided message.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The message handlers.</returns>
        public virtual IEnumerable<IMessageHandler<TMessage, TResult>> ResolveMessageHandlers<TMessage, TResult>(
            IMessagingContext context)
            where TMessage : IMessage<TResult>
        {
            var envelopeType = context.EnvelopeType;
            var messageType = context.MessageType;
            var (_, _, _, messageHandlersFactory) = this.handlerFactories.GetOrAdd($"{envelopeType}/{messageType}", _ =>
            {
                var handlerProvider = this.handlerProviders.FirstOrDefault(s => s.CanHandle(context));
                if (handlerProvider == null)
                {
                    return (envelopeType, messageType, messageId, () => null);
                }

                return (envelopeType, messageType, messageId, handlerProvider.GetHandlersFactory(this.handlerRegistry, envelopeType));
            });

            var handlers = messageHandlersFactory()?.OfType<IMessageHandler<TMessage, TResult>>();
            return handlers ?? Array.Empty<IMessageHandler<TMessage, TResult>>();
        }

        private void ResetFactoryCache(IMessageMatch messageMatch)
        {
            // remove all factories which match the metadata match.
            var factoriesToDelete = this.handlerFactories
                .Where(f => this.messageMatchService.IsMatch(messageMatch, f.Value.envelopeType, f.Value.messageType))
                .ToList();

            foreach (var factoryToDelete in factoriesToDelete)
            {
                this.handlerFactories.TryRemove(factoryToDelete.Key, out _);
            }
        }
    }
}
