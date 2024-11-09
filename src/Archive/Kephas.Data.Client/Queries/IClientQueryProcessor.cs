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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for client query processor.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IClientQueryProcessor
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
        Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            Action<IClientQueryExecutionContext> optionsConfig = null,
            CancellationToken cancellationToken = default);
    }
}