// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the query handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Client.Queries;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Message handler for <see cref="QueryMessage"/>.
    /// </summary>
    public class QueryHandler : MessageHandlerBase<QueryMessage, QueryResponseMessage>
    {
        /// <summary>
        /// The composition context.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// The client query executor.
        /// </summary>
        private readonly IClientQueryExecutor clientQueryExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandler"/> class.
        /// </summary>
        /// <param name="compositionContext">The dependecy injection/composition context.</param>
        /// <param name="clientQueryExecutor">The client query executor.</param>
        public QueryHandler(ICompositionContext compositionContext, IClientQueryExecutor clientQueryExecutor)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(clientQueryExecutor, nameof(clientQueryExecutor));

            this.compositionContext = compositionContext;
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
        public override async Task<QueryResponseMessage> ProcessAsync(
            QueryMessage message,
            IMessageProcessingContext context,
            CancellationToken token)
        {
            var executionContext = new ClientQueryExecutionContext(this.compositionContext);
            executionContext.Merge(message.Options);

            var clientEntities = await this.clientQueryExecutor.ExecuteQueryAsync(message.Query, executionContext, token)
                                     .PreserveThreadContext();

            return new QueryResponseMessage { Entities = clientEntities.ToArray() };
        }
    }
}