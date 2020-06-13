// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoQueryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo query provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Runtime;

namespace Kephas.Data.MongoDB.Linq
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;
    using Kephas.Reflection;

    using global::MongoDB.Driver.Linq;

    /// <summary>
    /// A mongo query provider.
    /// </summary>
    public class MongoQueryProvider : DataContextQueryProvider
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoQueryProvider"/> class.
        /// </summary>
        /// <param name="queryOperationContext">The query operation context.</param>
        /// <param name="nativeQueryProvider">The native query provider.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public MongoQueryProvider(IQueryOperationContext queryOperationContext, IQueryProvider nativeQueryProvider, IRuntimeTypeRegistry typeRegistry)
            : base(queryOperationContext, nativeQueryProvider)
        {
            this.typeRegistry = typeRegistry;
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
        public override Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default)
        {
            var nativeProviderTypeInfo = this.typeRegistry.GetTypeInfo(this.NativeQueryProvider.GetType());
            var executeAsyncMethodInfo = nativeProviderTypeInfo.Methods[nameof(this.ExecuteAsync)].Single();
            var executeAsync = executeAsyncMethodInfo.MethodInfo.MakeGenericMethod(typeof(object));
            var taskResult = (Task<object>)executeAsync.Call(this.NativeQueryProvider, expression, cancellationToken);
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
        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var nativeProviderTypeInfo = this.typeRegistry.GetTypeInfo(this.NativeQueryProvider.GetType());
            var executeAsyncMethodInfo = nativeProviderTypeInfo.Methods[nameof(this.ExecuteAsync)].Single();
            var executeAsync = executeAsyncMethodInfo.MethodInfo.MakeGenericMethod(typeof(TResult));
            var taskResult = (Task<TResult>)executeAsync.Call(this.NativeQueryProvider, expression, cancellationToken);
            return taskResult;
        }

        /// <summary>
        /// Prepares the provided expression for execution.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An expression prepared for execution.
        /// </returns>
        internal new Expression GetExecutableExpression(Expression expression)
        {
            return base.GetExecutableExpression(expression);
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query
        /// represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TElement">The query element type.</typeparam>
        /// <param name="nativeQuery">The native query.</param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the
        /// specified expression tree.
        /// </returns>
        protected override IQueryable<TElement> CreateQuery<TElement>(IQueryable<TElement> nativeQuery)
        {
            return new MongoQuery<TElement>(this, (IMongoQueryable<TElement>)nativeQuery, this.typeRegistry);
        }
    }
}