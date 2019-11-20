// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Provides the default implementation of the <see cref="IMessageProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageProcessor : Loggable, IMessageProcessor
    {
        private readonly IMessageHandlerRegistry handlerRegistry;
        private readonly IMessageMatchService messageMatchService;
        private readonly IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories;
        private readonly
            ConcurrentDictionary<string, (IEnumerable<IMessagingBehavior> behaviors, IEnumerable<IMessagingBehavior> reversedBehaviors)> behaviorFactoriesDictionary =
                new ConcurrentDictionary<string, (IEnumerable<IMessagingBehavior>, IEnumerable<IMessagingBehavior>)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProcessor" /> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="handlerRegistry">The handler registry.</param>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultMessageProcessor(
            IContextFactory contextFactory,
            IMessageHandlerRegistry handlerRegistry,
            IMessageMatchService messageMatchService,
            IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories)
            : base(contextFactory)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(handlerRegistry, nameof(handlerRegistry));
            Requires.NotNull(messageMatchService, nameof(messageMatchService));
            Requires.NotNull(behaviorFactories, nameof(behaviorFactories));

            this.ContextFactory = contextFactory;
            this.handlerRegistry = handlerRegistry;
            this.messageMatchService = messageMatchService;
            this.behaviorFactories = behaviorFactories.Order().ToList();
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IContextFactory ContextFactory { get; }

        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="message">The message to process.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the response message.
        /// </returns>
        public async Task<IMessage> ProcessAsync(IMessage message, Action<IMessagingContext> optionsConfig = null, CancellationToken token = default)
        {
            Requires.NotNull(message, nameof(message));

            var (behaviors, reversedBehaviors) = this.GetOrderedBehaviors(message);
            var localContext = this.CreateProcessingContext(message, optionsConfig);

            try
            {
                foreach (var messageHandler in this.handlerRegistry.ResolveMessageHandlers(message))
                {
                    using (messageHandler)
                    {
                        localContext.Message = message;
                        localContext.Handler = messageHandler;

                        try
                        {
                            await this.ApplyBeforeProcessBehaviorsAsync(behaviors, localContext, token)
                                .PreserveThreadContext();

                            var response = await messageHandler.ProcessAsync(message, localContext, token)
                                               .PreserveThreadContext();
                            localContext.Response = response;
                        }
                        catch (Exception ex)
                        {
                            localContext.Exception = ex;
                        }
                        finally
                        {
                            // restore the message and handler that could be changed
                            // by a nested message processor ProcessAsync call.
                            localContext.Handler = messageHandler;
                            localContext.Message = message;
                        }

                        await this.ApplyAfterProcessBehaviorsAsync(reversedBehaviors, localContext, token)
                            .PreserveThreadContext();
                    }
                }

                return localContext.Exception != null
                           ? throw localContext.Exception
                           : localContext.Response;
            }
            finally
            {
                localContext?.Dispose();
            }
        }

        /// <summary>
        /// Applies the behaviors invoking the <see cref="IMessagingBehavior.AfterProcessAsync"/> asynchronously.
        /// </summary>
        /// <param name="reversedBehaviors">The reversed behaviors.</param>
        /// <param name="context">Context for the message processing.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task ApplyAfterProcessBehaviorsAsync(
            IEnumerable<IMessagingBehavior> reversedBehaviors,
            IMessagingContext context,
            CancellationToken token)
        {
            foreach (var behavior in reversedBehaviors)
            {
                await behavior.AfterProcessAsync(context, token).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Applies the behaviors invoking the <see cref="IMessagingBehavior.BeforeProcessAsync"/> asynchronously.
        /// </summary>
        /// <param name="behaviors">The reversed behaviors.</param>
        /// <param name="context">Context for the message processing.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task ApplyBeforeProcessBehaviorsAsync(
            IEnumerable<IMessagingBehavior> behaviors,
            IMessagingContext context,
            CancellationToken token)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.BeforeProcessAsync(context, token).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Creates the processing context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <returns>
        /// The processing context.
        /// </returns>
        protected virtual IMessagingContext CreateProcessingContext(
            IMessage message,
            Action<IMessagingContext> optionsConfig)
        {
            var context = this.ContextFactory.CreateContext<MessagingContext>(this);
            optionsConfig?.Invoke(context);
            context.Message = message;
            return context;
        }

        /// <summary>
        /// Gets the ordered behaviors (direct and reversed) to be applied.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An ordered list of behaviors which can be applied to the provided message, with their reversed counterpart.
        /// </returns>
        protected virtual (IEnumerable<IMessagingBehavior> behaviors, IEnumerable<IMessagingBehavior> reversedBehaviors) GetOrderedBehaviors(IMessage message)
        {
            var messageType = this.messageMatchService.GetMessageType(message);
            var messageId = this.messageMatchService.GetMessageId(message);

            var orderedBehaviorsEntry = this.behaviorFactoriesDictionary.GetOrAdd(
                $"{message.GetType()}/{messageType}/{messageId}",
                _ =>
                    {
                        var behaviors = this.behaviorFactories.Where(
                                f => this.messageMatchService.IsMatch(f.Metadata.MessageMatch, message.GetType(), messageType, messageId))
                            .Select(f => f.CreateExportedValue())
                            .ToList();

                        return (behaviors, ((IEnumerable<IMessagingBehavior>)behaviors).Reverse().ToList());
                    });

            return orderedBehaviorsEntry;
        }
    }
}