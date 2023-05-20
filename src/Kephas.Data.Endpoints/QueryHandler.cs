﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the query handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Client.Queries;
    using Kephas.Messaging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Message handler for <see cref="QueryMessage"/>.
    /// </summary>
    public class QueryHandler : IMessageHandler<QueryMessage, QueryResponse>
    {
        private readonly IInjectableFactory injectableFactory;
        private readonly IClientQueryProcessor clientQueryExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandler"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="clientQueryExecutor">The client query executor.</param>
        public QueryHandler(IInjectableFactory injectableFactory, IClientQueryProcessor clientQueryExecutor)
        {
            injectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
            clientQueryExecutor = clientQueryExecutor ?? throw new System.ArgumentNullException(nameof(clientQueryExecutor));

            this.injectableFactory = injectableFactory;
            this.clientQueryExecutor = clientQueryExecutor;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public async Task<QueryResponse> ProcessAsync(
            QueryMessage message,
            IMessagingContext context,
            CancellationToken token)
        {
            var optionsConfig = this.GetQueryExecutionConfig(message, context);
            var clientEntities = await this.clientQueryExecutor.ExecuteQueryAsync(message.Query, optionsConfig, token)
                                     .PreserveThreadContext();

            return new QueryResponse { Entities = clientEntities.ToArray() };
        }

        /// <summary>
        /// Gets the query execution configuration options.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <returns>
        /// The query execution configuration options.
        /// </returns>
        protected virtual Action<IClientQueryExecutionContext> GetQueryExecutionConfig(
            QueryMessage message,
            IMessagingContext context)
        {
            return ctx => ctx.Impersonate(context).Options(message.Options);
        }
    }
}