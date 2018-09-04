﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// <param name="executionContext">Context for the execution (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A list of client entities.
        /// </returns>
        public Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            IClientQueryExecutionContext executionContext = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IList<object>>(null);
        }
    }
}