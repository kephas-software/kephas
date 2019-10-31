// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullClientQueryExecutor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null client query executor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null client query processor.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullClientQueryProcessor : IClientQueryProcessor
    {
        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="optionsConfig">Optional. The configuration options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the list of client entities.
        /// </returns>
        public Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            Action<IClientQueryExecutionContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IList<object>>(null);
        }
    }
}