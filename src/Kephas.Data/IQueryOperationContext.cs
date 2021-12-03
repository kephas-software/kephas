// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for query contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Contract for query contexts.
    /// </summary>
    public interface IQueryOperationContext : IDataOperationContext
    {
        /// <summary>
        /// Gets or sets the implementation type resolver.
        /// </summary>
        /// <value>
        /// The implementation type resolver.
        /// </value>
        Func<Type, IContext, Type> ImplementationTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        object Options { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        IQueryable Query { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IQueryOperationContext"/>.
    /// </summary>
    public static class QueryOperationContextExtensions
    {
        /// <summary>
        /// Sets the implementation type resolver.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the query operation context.</typeparam>
        /// <param name="queryContext">The query context.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>
        /// This <paramref name="queryContext"/>.
        /// </returns>
        public static TContext ImplementationTypeResolver<TContext>(this TContext queryContext, Func<Type, IContext, Type> resolver)
            where TContext : class, IQueryOperationContext
        {
            queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));

            queryContext.ImplementationTypeResolver = resolver;

            return queryContext;
        }

        /// <summary>
        /// Sets the options for controlling the operation.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the query operation context.</typeparam>
        /// <param name="queryContext">The query context.</param>
        /// <param name="options">Options for controlling the operation.</param>
        /// <returns>
        /// This <paramref name="queryContext"/>.
        /// </returns>
        public static TContext Options<TContext>(this TContext queryContext, object options)
            where TContext : class, IQueryOperationContext
        {
            queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));

            queryContext.Options = options;

            return queryContext;
        }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the query operation context.</typeparam>
        /// <param name="queryContext">The query context.</param>
        /// <param name="query">The query.</param>
        /// <returns>
        /// This <paramref name="queryContext"/>.
        /// </returns>
        public static TContext Query<TContext>(this TContext queryContext, IQueryable query)
            where TContext : class, IQueryOperationContext
        {
            queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));

            queryContext.Query = query;

            return queryContext;
        }
    }
}