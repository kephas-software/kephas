// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the default implementation of the <see cref="IMessageProcessor" /> application service contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.HandlerSelectors;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Provides the default implementation of the <see cref="IMessageProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageProcessor : IMessageProcessor
    {
        /// <summary>
        /// The handler selector factories.
        /// </summary>
        private readonly IList<IMessageHandlerSelector> handlerSelectors;

        /// <summary>
        /// The filter factories.
        /// </summary>
        private readonly IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories;

        /// <summary>
        /// The handler factories.
        /// </summary>
        private readonly ConcurrentDictionary<string, Func<IEnumerable<IMessageHandler>>> handlerFactories = new ConcurrentDictionary<string, Func<IEnumerable<IMessageHandler>>>();

        /// <summary>
        /// The handler factories.
        /// </summary>
        private readonly
            ConcurrentDictionary<string, (IEnumerable<IMessageProcessingFilter> filters, IEnumerable<IMessageProcessingFilter> reversedFilters)> filterFactoriesDictionary =
                new ConcurrentDictionary<string, (IEnumerable<IMessageProcessingFilter>, IEnumerable<IMessageProcessingFilter>)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProcessor" /> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="handlerSelectorFactories">The handler selector factories.</param>
        /// <param name="filterFactories">The filter factories.</param>
        public DefaultMessageProcessor(
            IAmbientServices ambientServices,
            IList<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>> handlerSelectorFactories,
            IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(handlerSelectorFactories, nameof(handlerSelectorFactories));
            Requires.NotNull(filterFactories, nameof(filterFactories));

            this.AmbientServices = ambientServices;
            this.handlerSelectors = handlerSelectorFactories
                .OrderBy(f => f.Metadata.ProcessingPriority)
                .Select(f => f.CreateExportedValue())
                .ToList();
            this.filterFactories = filterFactories
                .OrderBy(f => f.Metadata.ProcessingPriority)
                .ToList();
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultMessageProcessor> Logger { get; set; }

        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">Context for the message processing.</param>
        /// <param name="token">  The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public async Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context = null, CancellationToken token = default)
        {
            Requires.NotNull(message, nameof(message));

            (var filters, var reversedFilters) = this.GetOrderedFilters(message);

            var contextHandler = context?.Handler;
            var contextMessage = context?.Message;
            foreach (var messageHandler in this.ResolveMessageHandlers(message))
            {
                using (messageHandler)
                {
                    if (context == null)
                    {
                        context = this.CreateProcessingContext(message, messageHandler);
                    }
                    else
                    {
                        context.Message = message;
                        context.Handler = messageHandler;
                    }

                    try
                    {
                        foreach (var filter in filters)
                        {
                            await filter.BeforeProcessAsync(context, token).PreserveThreadContext();
                        }

                        var response = await messageHandler.ProcessAsync(message, context, token)
                                           .PreserveThreadContext();
                        context.Response = response;
                    }
                    catch (Exception ex)
                    {
                        context.Exception = ex;
                    }
                    finally
                    {
                        // restore the message and handler that could be changed
                        // by a nested message processor ProcessAsync call.
                        context.Handler = messageHandler;
                        context.Message = message;
                    }

                    foreach (var filter in reversedFilters)
                    {
                        await filter.AfterProcessAsync(context, token).PreserveThreadContext();
                    }
                }
            }

            if (context != null)
            {
                // restore the previous context handler and message.
                context.Handler = contextHandler;
                context.Message = contextMessage;
            }

            if (context.Exception != null)
            {
                throw context.Exception;
            }

            return context.Response;
        }

        /// <summary>
        /// Resolves the message handlers for the provided message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message handlers.</returns>
        protected virtual IEnumerable<IMessageHandler> ResolveMessageHandlers(IMessage message)
        {
            var messageType = this.GetMessageType(message);
            var messageName = this.GetMessageName(message);
            var messageHandlersFactory = this.handlerFactories.GetOrAdd($"{messageType.FullName}/{messageName}", _ =>
                {
                    var handlerSelector = this.handlerSelectors.FirstOrDefault(s => s.CanHandle(messageType, messageName));
                    if (handlerSelector == null)
                    {
                        return () => null;
                    }

                    return handlerSelector.GetHandlersFactory(messageType, messageName);
                });

            var handlers = messageHandlersFactory();
            return handlers ?? new IMessageHandler[0];
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The message type.
        /// </returns>
        protected virtual Type GetMessageType(IMessage message)
        {
            return message.GetType();
        }

        /// <summary>
        /// Gets the message name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The message name.
        /// </returns>
        protected virtual string GetMessageName(IMessage message)
        {
            var expandoMessage = message.ToExpando();
            var messageName = expandoMessage["MessageName"] as string;
            return messageName;
        }

        /// <summary>
        /// Creates the processing context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>The processing context.</returns>
        protected virtual IMessageProcessingContext CreateProcessingContext(IMessage message, IMessageHandler handler)
        {
            return new MessageProcessingContext(this, message, handler);
        }

        /// <summary>
        /// Gets the ordered filters (direct and reversed) to be applied.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An ordered list of filters which can be applied to the provided message, with their reversed counterpart.
        /// </returns>
        protected virtual (IEnumerable<IMessageProcessingFilter> filters, IEnumerable<IMessageProcessingFilter> reversedFilters) GetOrderedFilters(IMessage message)
        {
            var messageType = this.GetMessageType(message);
            var messageName = this.GetMessageName(message);

            var orderedFiltersEntry = this.filterFactoriesDictionary.GetOrAdd(
                $"{messageType.FullName}/{messageName}",
                _ =>
                    {
                        var orderedFilters = (from f in this.filterFactories
                                              where (f.Metadata.MessageType?.GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo()) ?? true)
                                                    && (f.Metadata.MessageName == null || messageName == f.Metadata.MessageName)
                                              select f.CreateExport().Value).ToList();

                        return (orderedFilters, ((IEnumerable<IMessageProcessingFilter>)orderedFilters).Reverse().ToList());
                    });

            return orderedFiltersEntry;
        }
    }
}