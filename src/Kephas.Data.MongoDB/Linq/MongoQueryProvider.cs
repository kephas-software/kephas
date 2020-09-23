// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoQueryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo query provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;
    using Kephas.Data.Linq;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A mongo query provider.
    /// </summary>
    public class MongoQueryProvider : DataContextQueryProvider
    {
        private static readonly MethodInfo ExecuteEnumerableAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((MongoQueryProvider)null!).ExecuteEnumerableAsync<string>(null!, default));
        private static readonly MethodInfo ExecuteScalarAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((MongoQueryProvider)null!).ExecuteScalarAsync<string>(null!, default));

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
        public override async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default)
        {
            var entityType = expression.Type.TryGetQueryableItemType();
            if (entityType != null)
            {
                // vector value
                var executeAsync = ExecuteEnumerableAsyncMethod.MakeGenericMethod(entityType);
                var executeTask = (Task<object>)executeAsync.Call(this, expression, cancellationToken);
                return await executeTask.PreserveThreadContext();
            }
            else
            {
                // scalar value
                var executeAsync = ExecuteScalarAsyncMethod.MakeGenericMethod(expression.Type);
                var executeTask = (Task<object>)executeAsync.Call(this, expression, cancellationToken);
                return await executeTask.PreserveThreadContext();
            }
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
        public override async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = await this.ExecuteAsync(expression, cancellationToken).PreserveThreadContext();
            return (TResult)result;
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

        private async Task<object> ExecuteEnumerableAsync<T>(Expression expression, CancellationToken cancellationToken)
        {
            var nativeProviderTypeInfo = this.typeRegistry.GetTypeInfo(this.NativeQueryProvider.GetType());
            var executeAsyncMethodInfo = nativeProviderTypeInfo.Methods["ExecuteAsync"].Single();
            var executeAsync = executeAsyncMethodInfo.MethodInfo.MakeGenericMethod(typeof(IAsyncCursor<T>));
            var executeAsyncTask = (Task<IAsyncCursor<T>>)executeAsync.Call(this.NativeQueryProvider, expression, cancellationToken);
            var asyncCursor = await executeAsyncTask.PreserveThreadContext();

            var dataContext = this.DataContext;
            var items = new List<T>();
            while (await asyncCursor.MoveNextAsync(cancellationToken).PreserveThreadContext())
            {
                items.AddRange(asyncCursor.Current.Select(e => (T)dataContext.Attach(e!).Entity));
            }

            return items;
        }

        private async Task<object?> ExecuteScalarAsync<T>(Expression expression, CancellationToken cancellationToken)
        {
            var nativeProviderTypeInfo = this.typeRegistry.GetTypeInfo(this.NativeQueryProvider.GetType());
            var executeAsyncMethodInfo = nativeProviderTypeInfo.Methods["ExecuteAsync"].Single();
            var executeAsync = executeAsyncMethodInfo.MethodInfo.MakeGenericMethod(typeof(T));
            var executeAsyncTask = (Task<T>)executeAsync.Call(this.NativeQueryProvider, expression, cancellationToken);
            var value = await executeAsyncTask.PreserveThreadContext();

            var valueType = value?.GetType();
            if (value == null
                || valueType!.IsValueType
                || value is string
                || !this.typeRegistry.GetTypeInfo(valueType!).Properties.ContainsKey(nameof(IIdentifiable.Id)))
            {
                return value;
            }

            return this.DataContext.Attach(value).Entity;
        }
    }
}