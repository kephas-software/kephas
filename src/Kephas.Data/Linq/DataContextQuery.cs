// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQuery.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context query class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data context query.
    /// </summary>
    /// <typeparam name="T">The query element type.</typeparam>
    public class DataContextQuery<T> : IOrderedQueryable<T>
    {
        /// <summary>
        /// The query provider.
        /// </summary>
        private readonly IQueryProvider queryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextQuery{T}"/> class.
        /// </summary>
        /// <param name="queryProvider">The query provider.</param>
        /// <param name="nativeQuery">The native query.</param>
        public DataContextQuery(IQueryProvider queryProvider, IQueryable<T> nativeQuery)
        {
            Requires.NotNull(queryProvider, nameof(queryProvider));
            Requires.NotNull(nativeQuery, nameof(nativeQuery));

            this.queryProvider = queryProvider;
            this.NativeQuery = nativeQuery;
        }

        /// <summary>
        /// Gets the native query.
        /// </summary>
        public virtual IQueryable<T> NativeQuery { get; }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with
        /// this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Type" /> that represents the type of the element(s) that are returned
        /// when the expression tree associated with this object is executed.
        /// </value>
        public Type ElementType => this.NativeQuery.ElementType;

        /// <summary>
        /// Gets the expression tree that is associated with the instance of
        /// <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this instance
        /// of <see cref="T:System.Linq.IQueryable" />.
        /// </value>
        public Expression Expression => this.NativeQuery.Expression;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data source.
        /// </value>
        public IQueryProvider Provider => this.queryProvider;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.queryProvider.Execute<IEnumerator<T>>(this.Expression);
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
    }
}