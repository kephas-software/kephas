﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the default implementation of the <see cref="IMessageProcessor" /> application service contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Pipelines;

namespace Kephas.Messaging
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Logging;
    using Kephas.Messaging.Behaviors;
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
        private readonly IServiceProvider serviceProvider;
        private readonly ConcurrentDictionary<string, IList<IMessagingBehavior>> behaviorFactoriesDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProcessor" /> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="handlerRegistry">The handler registry.</param>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public DefaultMessageProcessor(
            IInjectableFactory injectableFactory,
            IMessageHandlerRegistry handlerRegistry,
            IMessageMatchService messageMatchService,
            IServiceProvider serviceProvider)
            : base(injectableFactory)
        {
            this.InjectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
            this.handlerRegistry = handlerRegistry ?? throw new System.ArgumentNullException(nameof(handlerRegistry));
            this.messageMatchService = messageMatchService ?? throw new System.ArgumentNullException(nameof(messageMatchService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets the injectable factory.
        /// </summary>
        /// <value>
        /// The injectable factory.
        /// </value>
        public IInjectableFactory InjectableFactory { get; }

        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="message">The message to process.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="token">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the response message.
        /// </returns>
        public async Task<TResult> ProcessAsync<TMessage, TResult>(
            TMessage message,
            Action<IMessagingContext<TMessage, TResult>>? optionsConfig = null,
            CancellationToken token = default)
            where TMessage : IMessage<TResult>
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            var exceptions = new List<Exception>();
            object? result = null;
            foreach (var messageHandler in this.handlerRegistry.ResolveMessageHandlers<TMessage, TResult>(message))
            {
                try
                {
                    result = await ProcessCoreAsync(message, messageHandler, () => this.CreateProcessingContext(message, optionsConfig), token).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            return exceptions.Count switch
            {
                0 => result,
                1 => throw exceptions[0],
                _ => throw new AggregateException(exceptions)
            };
        }

        /// <summary>
        /// Processes the message asynchronously using the handler, behaviors, and the messaging context.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="contextFactory">The messaging context factory.</param>
        /// <param name="token">The cancellation token.</param>
        protected virtual async Task<object?> ProcessCoreAsync<TMessage, TResult>(
            TMessage message,
            IMessageHandler messageHandler,
            Func<IMessagingContext> contextFactory,
            Func<IPipeline<IMessageProcessor>
            CancellationToken token)
            where TMessage : IMessage<TResult>
        {
            using var context = contextFactory();

            using var behaviorEnumerator = behaviors.GetEnumerator();
            Func<Task<object?>>? next = null;
            next = async () =>
            {
                if (behaviorEnumerator.MoveNext())
                {
                    return await behaviorEnumerator.Current
                        .ProcessAsync(next!, context, token)
                        .PreserveThreadContext();
                }

                return await messageHandler
                    .ProcessAsync(message, context, token)
                    .PreserveThreadContext();
            };

            return await next().PreserveThreadContext();
        }

        /// <summary>
        /// Creates the processing context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <returns>
        /// The processing context.
        /// </returns>
        protected virtual IMessagingContext<TMessage, TResult> CreateProcessingContext<TMessage, TResult>(
            TMessage message,
            Action<IMessagingContext<TMessage, TResult>>? optionsConfig)
            where TMessage : IMessage<TResult>
        {
            var context = this.InjectableFactory.Create<MessagingContext<TMessage, TResult>>(message);
            optionsConfig?.Invoke(context);
            return context;
        }

        /// <summary>
        /// Gets the ordered behaviors (direct and reversed) to be applied.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An ordered list of behaviors which can be applied to the provided message, with their reversed counterpart.
        /// </returns>
        protected virtual IList<IMessagingBehavior> GetOrderedBehaviors<>(IMessage message)
        {
            var messageType = this.messageMatchService.GetMessageType(message);
            var messageId = this.messageMatchService.GetMessageId(message);

            var orderedBehaviors = this.behaviorFactoriesDictionary.GetOrAdd(
                $"{message.GetType()}/{messageType}/{messageId}",
                _ =>
                    {
                        var behaviors = this.behaviorFactories.Where(
                                f => this.messageMatchService.IsMatch(f.Metadata.MessageMatch, message.GetType(), messageType, messageId))
                            .Select(f => f.CreateExportedValue())
                            .ToList();

                        return behaviors;
                    });

            return orderedBehaviors;
        }
    }
}