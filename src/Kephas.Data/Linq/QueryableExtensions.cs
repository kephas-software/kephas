// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the queryable extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Executes the query asynchronously and returns a list of items.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of items.</returns>
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(query, nameof(query));

            var asyncProvider = query.Provider as IAsyncQueryProvider;
            if (asyncProvider != null)
            {
                return asyncProvider.ExecuteAsync<List<T>>(query.Expression, cancellationToken);
            }

            return Task.FromResult(query.ToList());
        }

        /// <summary>
        /// Executes the query asynchronously and returns a list of items.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of items.</returns>
        public static Task<List<object>> ToListAsync(this IQueryable query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ToListAsync<object>((IQueryable<object>)query, cancellationToken);
        }
    }
}