// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoQuery.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo query class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    /// <summary>
    /// A mongo query adapter.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class MongoQuery<T> : IMongoQueryable<T>
    {
        /// <summary>
        /// The query provider.
        /// </summary>
        private readonly MongoQueryProvider queryProvider;

        /// <summary>
        /// The native query.
        /// </summary>
        private readonly IMongoQueryable<T> nativeQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoQuery{T}"/> class.
        /// </summary>
        /// <param name="queryProvider">The query provider.</param>
        /// <param name="nativeQuery">The native query.</param>
        public MongoQuery(MongoQueryProvider queryProvider, IMongoQueryable<T> nativeQuery)
        {
            Requires.NotNull(queryProvider, nameof(queryProvider));
            Requires.NotNull(nativeQuery, nameof(nativeQuery));

            this.queryProvider = queryProvider;
            this.nativeQuery = nativeQuery;
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with
        /// this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Type" /> that represents the type of the element(s) that are returned
        /// when the expression tree associated with this object is executed.
        /// </value>
        public Type ElementType => this.nativeQuery.ElementType;

        /// <summary>
        /// Gets the expression tree that is associated with the instance of
        /// <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this instance
        /// of <see cref="T:System.Linq.IQueryable" />.
        /// </value>
        public Expression Expression => this.nativeQuery.Expression;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data source.
        /// </value>
        public IQueryProvider Provider => this.queryProvider;

        /// <summary>Gets the execution model.</summary>
        /// <returns>The execution model.</returns>
        public QueryableExecutionModel GetExecutionModel()
        {
            return this.nativeQuery.GetExecutionModel();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.nativeQuery.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Executes the operation and returns a cursor to the results.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A cursor.</returns>
        public IAsyncCursor<T> ToCursor(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.nativeQuery.ToCursor(cancellationToken);
        }

        /// <summary>
        /// Executes the operation and returns a cursor to the results.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is a cursor.</returns>
        public Task<IAsyncCursor<T>> ToCursorAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.nativeQuery.ToCursorAsync(cancellationToken);
        }
    }
}