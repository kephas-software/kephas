// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryExecutor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    [SharedAppServiceContract]
    public interface IClientQueryExecutor
    {
        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A list of client entities.
        /// </returns>
        Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}