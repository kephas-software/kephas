// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides the default implementation of the <see cref="IMessageProcessor" /> application service contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Messaging.Server.Composition;
    using Kephas.Services;

    /// <summary>
    /// Provides the default implementation of the <see cref="IMessageProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageProcessor : IMessageProcessor
    {
        /// <summary>
        /// The filter factories.
        /// </summary>
        private readonly IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProcessor" /> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="filterFactories">The filter factories.</param>
        public DefaultMessageProcessor(ICompositionContext compositionContext, IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories)
        {
            Contract.Requires(compositionContext != null);
            Contract.Requires(filterFactories != null);

            this.CompositionContext = compositionContext;
            this.filterFactories = filterFactories;
        }

        /// <summary>
        /// Gets the composition context.
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
        public async Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context = null, CancellationToken token = default(CancellationToken))
        {
            using (var messageHandler = this.CreateMessageHandler(message))
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

                var filters = this.GetOrderedFilters(context);

                try
                {
                    foreach (var filter in filters)
                    {
                        await filter.BeforeProcessAsync(context, token);
                    }

                    var response = await messageHandler.ProcessAsync(message, context, token);
                    context.Response = response;
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }

                foreach (var filter in filters.Reverse())
                {
                    await filter.AfterProcessAsync(context, token);
                }

                if (context.Exception != null)
                {
                    throw context.Exception;
                }

                return context.Response;
            }
        }

        /// <summary>
        /// Creates the message handler.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The newly created message handler.</returns>
        protected virtual IMessageHandler CreateMessageHandler(IMessage message)
        {
            var messageHandlerType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
            var messageHandler = (IMessageHandler)this.CompositionContext.GetExport(messageHandlerType);
            return messageHandler;
        }

        /// <summary>
        /// Creates the processing context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>The processing context.</returns>
        protected virtual IMessageProcessingContext CreateProcessingContext(IMessage message, IMessageHandler handler)
        {
            Contract.Ensures(Contract.Result<IMessageProcessingContext>() != null);

            return new MessageProcessingContext(message, handler);
        }

        /// <summary>
        /// Gets the ordered filters to be applied.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An ordered list of filters which can be applied to the provided context.</returns>
        protected virtual IList<IMessageProcessingFilter> GetOrderedFilters(IMessageProcessingContext context)
        {
            Contract.Ensures(Contract.Result<IList<IMessageProcessingFilter>>() != null);

            var requestTypeInfo = context.Message.GetType().GetTypeInfo();
            var behaviors = (from b in this.filterFactories
                             where b.Metadata.MessageType.GetTypeInfo().IsAssignableFrom(requestTypeInfo)
                             orderby b.Metadata.ProcessingPriority
                             select b.CreateExport().Value).ToList();

            // TODO optimize to cache the ordered filters/message type.
            return behaviors;
        }
    }
}