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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Behaviors.Composition;
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
        /// The message match service.
        /// </summary>
        private readonly IMessageMatchService messageMatchService;

        /// <summary>
        /// The handler selector factories.
        /// </summary>
        private readonly IList<IMessageHandlerSelector> handlerSelectors;

        /// <summary>
        /// The behavior factories.
        /// </summary>
        private readonly IList<IExportFactory<IMessageProcessingBehavior, MessageProcessingBehaviorMetadata>> behaviorFactories;

        /// <summary>
        /// The handler factories.
        /// </summary>
        private readonly ConcurrentDictionary<string, Func<IEnumerable<IMessageHandler>>> handlerFactories = new ConcurrentDictionary<string, Func<IEnumerable<IMessageHandler>>>();

        /// <summary>
        /// The behavior factories.
        /// </summary>
        private readonly
            ConcurrentDictionary<string, (IEnumerable<IMessageProcessingBehavior> behaviors, IEnumerable<IMessageProcessingBehavior> reversedBehaviors)> behaviorFactoriesDictionary =
                new ConcurrentDictionary<string, (IEnumerable<IMessageProcessingBehavior>, IEnumerable<IMessageProcessingBehavior>)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProcessor" /> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="handlerSelectorFactories">The handler selector factories.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultMessageProcessor(
            ICompositionContext compositionContext,
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>> handlerSelectorFactories,
            IList<IExportFactory<IMessageProcessingBehavior, MessageProcessingBehaviorMetadata>> behaviorFactories)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(messageMatchService, nameof(messageMatchService));
            Requires.NotNull(handlerSelectorFactories, nameof(handlerSelectorFactories));
            Requires.NotNull(behaviorFactories, nameof(behaviorFactories));

            this.CompositionContext = compositionContext;
            this.messageMatchService = messageMatchService;
            this.handlerSelectors = handlerSelectorFactories
                .OrderBy(f => f.Metadata.ProcessingPriority)
                .Select(f => f.CreateExportedValue())
                .ToList();
            this.behaviorFactories = behaviorFactories
                .OrderBy(f => f.Metadata.ProcessingPriority)
                .ToList();
        }

        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        public ICompositionContext CompositionContext { get; }

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

            var (behaviors, reversedBehaviors) = this.GetOrderedBehaviors(message);

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
                        foreach (var behavior in behaviors)
                        {
                            await behavior.BeforeProcessAsync(context, token).PreserveThreadContext();
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

                    foreach (var behavior in reversedBehaviors)
                    {
                        await behavior.AfterProcessAsync(context, token).PreserveThreadContext();
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
            var messageType = this.messageMatchService.GetMessageType(message);
            var messageId = this.messageMatchService.GetMessageId(message);
            var messageHandlersFactory = this.handlerFactories.GetOrAdd($"{messageType.FullName}/{messageId}", _ =>
                {
                    var handlerSelector = this.handlerSelectors.FirstOrDefault(s => s.CanHandle(messageType, messageId));
                    if (handlerSelector == null)
                    {
                        return () => null;
                    }

                    return handlerSelector.GetHandlersFactory(messageType, messageId);
                });

            var handlers = messageHandlersFactory();
            return handlers ?? new IMessageHandler[0];
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
        /// Gets the ordered behaviors (direct and reversed) to be applied.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An ordered list of behaviors which can be applied to the provided message, with their reversed counterpart.
        /// </returns>
        protected virtual (IEnumerable<IMessageProcessingBehavior> behaviors, IEnumerable<IMessageProcessingBehavior> reversedBehaviors) GetOrderedBehaviors(IMessage message)
        {
            var messageType = this.messageMatchService.GetMessageType(message);
            var messageId = this.messageMatchService.GetMessageId(message);

            var orderedBehaviorsEntry = this.behaviorFactoriesDictionary.GetOrAdd(
                $"{messageType.FullName}/{messageId}",
                _ =>
                    {
                        var behaviors = this.behaviorFactories.Where(
                                f => this.messageMatchService.IsMatch(f.Metadata.MessageMatch, messageType, messageId))
                            .Select(f => f.CreateExportedValue())
                            .ToList();

                        return (behaviors, ((IEnumerable<IMessageProcessingBehavior>)behaviors).Reverse().ToList());
                    });

            return orderedBehaviorsEntry;
        }
    }
}