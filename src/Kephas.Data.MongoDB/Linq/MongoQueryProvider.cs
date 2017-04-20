// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoQueryProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo query provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Linq
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    using global::MongoDB.Driver.Linq;

    /// <summary>
    /// A mongo query provider.
    /// </summary>
    public class MongoQueryProvider : IAsyncQueryProvider
    {
        /// <summary>
        /// The Mongo data context.
        /// </summary>
        private readonly MongoDataContext dataContext;

        /// <summary>
        /// The native query provider.
        /// </summary>
        private readonly IQueryProvider nativeQueryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoQueryProvider"/> class.
        /// </summary>
        /// <param name="dataContext">The Mongo data context.</param>
        /// <param name="nativeQueryProvider">The native query provider.</param>
        public MongoQueryProvider(MongoDataContext dataContext, IQueryProvider nativeQueryProvider)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(nativeQueryProvider, nameof(nativeQueryProvider));

            this.dataContext = dataContext;
            this.nativeQueryProvider = nativeQueryProvider;
        }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public IQueryable CreateQuery(Expression expression)
        {
            Requires.NotNull(expression, nameof(expression));

            var elementType = expression.Type.TryGetEnumerableItemType();
            var nativeQuery = this.nativeQueryProvider.CreateQuery(expression);
            var mongoQueryTypeInfo = typeof(MongoQuery<>).MakeGenericType(elementType).AsRuntimeTypeInfo();
            var mongoQuery = (IQueryable)mongoQueryTypeInfo.CreateInstance(new object[] { this, nativeQuery });
            return mongoQuery;
        }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable`1" /> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TElement">The type of the elements of the <see cref="T:System.Linq.IQueryable`1" /> that is returned.</typeparam>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var nativeQuery = (IMongoQueryable<TElement>)this.nativeQueryProvider.CreateQuery<TElement>(expression);
            return new MongoQuery<TElement>(this, nativeQuery);
        }

        /// <summary>Executes the query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public object Execute(Expression expression)
        {
            return this.nativeQueryProvider.Execute(expression);
        }

        /// <summary>Executes the strongly-typed query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        public TResult Execute<TResult>(Expression expression)
        {
            return this.nativeQueryProvider.Execute<TResult>(expression);
        }

        /// <summary>
        /// Asynchronously executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression"> An expression tree that represents a LINQ query. </param>
        /// <param name="cancellationToken">
        /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the value that results from executing the specified query.
        /// </returns>
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            var nativeProviderTypeInfo = this.nativeQueryProvider.GetType().AsRuntimeTypeInfo();
            var executeAsyncMethodInfo = nativeProviderTypeInfo.Methods[nameof(this.ExecuteAsync)].Single();
            var executeAsync = executeAsyncMethodInfo.MethodInfo.MakeGenericMethod(typeof(object));
            var taskResult = (Task<object>)executeAsync.Call(this.nativeQueryProvider, expression, cancellationToken);
            return taskResult;
        }

        /// <summary>
        /// Asynchronously executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TResult"> The type of the value that results from executing the query. </typeparam>
        /// <param name="expression"> An expression tree that represents a LINQ query. </param>
        /// <param name="cancellationToken">
        /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the value that results from executing the specified query.
        /// </returns>
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            var nativeProviderTypeInfo = this.nativeQueryProvider.GetType().AsRuntimeTypeInfo();
            var executeAsyncMethodInfo = nativeProviderTypeInfo.Methods[nameof(this.ExecuteAsync)].Single();
            var executeAsync = executeAsyncMethodInfo.MethodInfo.MakeGenericMethod(typeof(TResult));
            var taskResult = (Task<TResult>)executeAsync.Call(this.nativeQueryProvider, expression, cancellationToken);
            return taskResult;
        }
    }
}