// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context query provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Activation;
    using Kephas.Data.Linq.Expressions;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A data context query provider base.
    /// </summary>
    public class DataContextQueryProvider : IDataContextQueryProvider
    {
        /// <summary>
        /// The generic method of IQueryable.CreateQuery{TElement}.
        /// </summary>
        private static readonly MethodInfo CreateQueryMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((IQueryProvider)null).CreateQuery<int>(null));

        /// <summary>
        /// The generic method of IQueryable.Execute{TResult}.
        /// </summary>
        private static readonly MethodInfo ExecuteMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((IQueryProvider)null).Execute<int>(null));

        /// <summary>
        /// The generic method of DataContextQueryProvider.ExecuteAsync{TResult}.
        /// </summary>
        private static readonly MethodInfo ExecuteAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((DataContextQueryProvider)null).ExecuteAsync<int>(null, default));

        /// <summary>
        /// The attach entity collection method.
        /// </summary>
        private static readonly MethodInfo AttachEntityCollectionMethod =
            ReflectionHelper.GetGenericMethodOf(
                _ => ((DataContextQueryProvider)null).AttachEntityCollection<IEnumerable<int>, int>(new int[0]));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextQueryProvider"/> class.
        /// </summary>
        /// <param name="queryOperationContext">The query operation context.</param>
        /// <param name="nativeQueryProvider">The native query provider.</param>
        public DataContextQueryProvider(IQueryOperationContext queryOperationContext, IQueryProvider nativeQueryProvider)
        {
            queryOperationContext = queryOperationContext ?? throw new System.ArgumentNullException(nameof(queryOperationContext));
            if (queryOperationContext.DataContext == null) throw new System.ArgumentNullException(nameof(queryOperationContext.DataContext));
            nativeQueryProvider = nativeQueryProvider ?? throw new System.ArgumentNullException(nameof(nativeQueryProvider));

            this.QueryOperationContext = queryOperationContext;
            this.DataContext = queryOperationContext.DataContext;
            this.NativeQueryProvider = nativeQueryProvider;
        }

        /// <summary>
        /// Gets the bound data context.
        /// </summary>
        /// <value>
        /// The bound data context.
        /// </value>
        public IDataContext DataContext { get; }

        /// <summary>
        /// Gets an operation context for the query.
        /// </summary>
        /// <value>
        /// The query operation context.
        /// </value>
        public IQueryOperationContext QueryOperationContext { get; }

        /// <summary>
        /// Gets the native query provider.
        /// </summary>
        /// <value>
        /// The native query provider.
        /// </value>
        public IQueryProvider NativeQueryProvider { get; }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query
        /// represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the
        /// specified expression tree.
        /// </returns>
        public virtual IQueryable CreateQuery(Expression expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            var elementType = expression.Type.TryGetEnumerableItemType();
            var createQuery = CreateQueryMethod.MakeGenericMethod(elementType);
            return (IQueryable)createQuery.Call(this, expression);
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable`1" /> object that can evaluate the query
        /// represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of the
        ///                            <see cref="T:System.Linq.IQueryable`1" /> that is returned.</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable`1" /> that can evaluate the query represented by the
        /// specified expression tree.
        /// </returns>
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            var nativeQuery = this.NativeQueryProvider.CreateQuery<TElement>(expression);
            return this.CreateQuery(nativeQuery);
        }

        /// <summary>Executes the query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public virtual object Execute(Expression expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            var expressionType = expression.Type;
            var expressionElementType = expressionType.TryGetEnumerableItemType();
            if (expressionElementType != null)
            {
                expressionType = typeof(IEnumerable<>).MakeGenericType(expressionElementType);
            }

            var execute = ExecuteMethod.MakeGenericMethod(expressionType);
            return execute.Call(this, expression);
        }

        /// <summary>Executes the strongly-typed query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        public virtual TResult Execute<TResult>(Expression expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            var executionResult = this.NativeQueryProvider.Execute<TResult>(this.GetExecutableExpression(expression));
            return this.AttachEntitiesToDataContext(executionResult);
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
        public virtual async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));

            var expressionType = expression.Type;
            var expressionElementType = expressionType.TryGetEnumerableItemType();
            if (expressionElementType != null)
            {
                expressionType = typeof(IEnumerable<>).MakeGenericType(expressionElementType);
            }

            var executeAsync = ExecuteAsyncMethod.MakeGenericMethod(expressionType);
            var typedTask = (Task)executeAsync.Call(this, expression, cancellationToken);
            await typedTask.PreserveThreadContext();
            return typedTask.GetPropertyValue(nameof(Task<int>.Result));
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
        public virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Execute<TResult>(expression));
        }

        /// <summary>
        /// Tries to get the data context's entity activator.
        /// </summary>
        /// <returns>
        /// An IActivator.
        /// </returns>
        protected internal virtual IActivator? TryGetEntityActivator()
        {
            return (this.DataContext as DataContextBase)?.EntityActivator;
        }

        /// <summary>
        /// Prepares the provided expression for execution.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An expression prepared for execution.
        /// </returns>
        protected internal virtual Expression GetExecutableExpression(Expression expression)
        {
            var implementationTypeResolver = this.QueryOperationContext?.ImplementationTypeResolver;
            var entityActivator = this.TryGetEntityActivator();

            if (entityActivator == null && implementationTypeResolver == null)
            {
                return expression;
            }

            var expressionVisitor = this.CreateQueryExpressionVisitor(implementationTypeResolver, entityActivator);
            expression = expressionVisitor.Visit(expression);
            return expression;
        }

        /// <summary>
        /// Creates an expression visitor used to visit the query expression.
        /// </summary>
        /// <param name="implementationTypeResolver">The implementation type resolver.</param>
        /// <param name="activator">The activator.</param>
        /// <returns>
        /// The new query expression visitor.
        /// </returns>
        protected virtual ExpressionVisitor CreateQueryExpressionVisitor(Func<Type, IContext, Type>? implementationTypeResolver, IActivator? activator)
        {
            return new SubstituteTypeExpressionVisitor(implementationTypeResolver, activator, context: this.QueryOperationContext);
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
        protected virtual IQueryable<TElement> CreateQuery<TElement>(IQueryable<TElement> nativeQuery)
        {
            return new DataContextQuery<TElement>(this, nativeQuery);
        }

        /// <summary>
        /// Attach entities to data context.
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        /// <param name="executionResult">The execution result.</param>
        /// <returns>
        /// The result containing attached entities.
        /// </returns>
        protected virtual TResult AttachEntitiesToDataContext<TResult>(TResult executionResult)
        {
            if (object.Equals(executionResult, default))
            {
                return default;
            }

            var enumerableItemType = executionResult.GetType().TryGetEnumerableItemType();
            if (enumerableItemType != null && this.IsAttachableType(enumerableItemType))
            {
                var enumerableResultItemType = typeof(TResult).TryGetEnumerableItemType();
                var attachEntityCollection = AttachEntityCollectionMethod.MakeGenericMethod(typeof(TResult), enumerableResultItemType);
                return (TResult)attachEntityCollection.Call(this, executionResult);
            }

            if (this.IsAttachable(executionResult))
            {
                return (TResult)this.DataContext.Attach(executionResult).Entity;
            }

            return executionResult;
        }

        /// <summary>
        /// Attaches an entity collection.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="executionResult">The execution result.</param>
        /// <returns>
        /// A list of attached entities.
        /// </returns>
        protected TResult AttachEntityCollection<TResult, T>(TResult executionResult)
            where TResult : IEnumerable<T>
        {
            var attachedList = new List<T>();
            foreach (var entity in executionResult)
            {
                if (this.IsAttachable(entity))
                {
                    attachedList.Add((T)this.DataContext.Attach(entity).Entity);
                }
                else
                {
                    attachedList.Add(entity);
                }
            }

            return this.ToExecutionResult<TResult, T>(attachedList);
        }

        /// <summary>
        /// Converts the list of attached entities to an execution result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="attachedList">List of attached entities.</param>
        /// <returns>
        /// The attached entity list as a TResult.
        /// </returns>
        protected virtual TResult ToExecutionResult<TResult, T>(IList<T> attachedList)
            where TResult : IEnumerable<T>
        {
            return (TResult)attachedList;
        }

        /// <summary>
        /// Indicates whether an entity is attachable.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// True if the entity is attachable, false if not.
        /// </returns>
        protected virtual bool IsAttachable(object entity)
        {
            return entity is IIdentifiable;
        }

        /// <summary>
        /// Indicates whether an entity type is attachable.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>
        /// True if the entity type is attachable, false if not.
        /// </returns>
        protected virtual bool IsAttachableType(Type entityType)
        {
            return typeof(IIdentifiable).GetTypeInfo().IsAssignableFrom(entityType.GetTypeInfo());
        }
    }
}