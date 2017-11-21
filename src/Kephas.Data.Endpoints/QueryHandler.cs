// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using Kephas.Data.Client.Queries;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Message handler for <see cref="QueryMessage"/>.
    /// </summary>
    public class QueryHandler : MessageHandlerBase<QueryMessage, QueryResponseMessage>
    {
        /// <summary>
        /// The client query executor.
        /// </summary>
        private readonly IClientQueryExecutor clientQueryExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandler"/> class.
        /// </summary>
        /// <param name="clientQueryExecutor">The client query executor.</param>
        public QueryHandler(IClientQueryExecutor clientQueryExecutor)
        {
            Requires.NotNull(clientQueryExecutor, nameof(clientQueryExecutor));

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
            var clientEntities = await this.clientQueryExecutor.ExecuteQueryAsync(message.Query, null, token)
                                     .PreserveThreadContext();

            return new QueryResponseMessage { Entities = clientEntities.ToArray() };
        }
    }
}