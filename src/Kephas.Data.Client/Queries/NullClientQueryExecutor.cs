// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullClientQueryExecutor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null client query executor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null client query executor.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullClientQueryExecutor : IClientQueryExecutor
    {
        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A list of client entities.
        /// </returns>
        public Task<IList<object>> ExecuteQueryAsync(ClientQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IList<object>>(null);
        }
    }
}