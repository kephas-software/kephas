// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryExecutor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IClientQueryExecutor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for client query executor.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IClientQueryExecutor
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
        Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            IClientQueryExecutionContext executionContext = null,
            CancellationToken cancellationToken = default);
    }
}