// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

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
        /// Initializes a new instance of the <see cref="DataContextQueryProvider"/> class.
        /// </summary>
        /// <param name="queryOperationContext">The query operation context.</param>
        /// <param name="nativeQueryProvider">The native query provider.</param>
        public DataContextQueryProvider(IQueryOperationContext queryOperationContext, IQueryProvider nativeQueryProvider)
        {
            Requires.NotNull(queryOperationContext, nameof(queryOperationContext));
            Requires.NotNull(queryOperationContext.DataContext, nameof(queryOperationContext.DataContext));
            Requires.NotNull(nativeQueryProvider, nameof(nativeQueryProvider));

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

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public virtual IQueryable CreateQuery(Expression expression)
        {
            Requires.NotNull(expression, nameof(expression));

            var elementType = expression.Type.TryGetEnumerableItemType();
            var createQuery = CreateQueryMethod.MakeGenericMethod(elementType);
            return (IQueryable)createQuery.Call(this, expression);
        }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable`1" /> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TElement">The type of the elements of the <see cref="T:System.Linq.IQueryable`1" /> that is returned.</typeparam>
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            Requires.NotNull(expression, nameof(expression));

            var nativeQuery = this.NativeQueryProvider.CreateQuery<TElement>(expression);
            return this.CreateQuery(nativeQuery);
        }

        /// <summary>Executes the query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public virtual object Execute(Expression expression)
        {
            Requires.NotNull(expression, nameof(expression));

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
            Requires.NotNull(expression, nameof(expression));

            var executionResult = this.NativeQueryProvider.Execute<TResult>(this.GetExecutableExpression(expression));
            this.AttachEntitiesToDataContext(executionResult);
            return executionResult;
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
        public virtual Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Execute(expression));
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
        protected internal virtual IActivator TryGetEntityActivator()
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
            var entityActivator = this.TryGetEntityActivator();
            if (entityActivator == null)
            {
                return expression;
            }

            var expressionVisitor = this.CreateQueryExpressionVisitor(entityActivator);
            expression = expressionVisitor.Visit(expression);
            return expression;
        }

        /// <summary>
        /// Creates an expression visitor used to visit the query expression.
        /// </summary>
        /// <param name="activator">The activator.</param>
        /// <returns>
        /// The new query expression visitor.
        /// </returns>
        protected virtual ExpressionVisitor CreateQueryExpressionVisitor(IActivator activator)
        {
            return new SubstituteTypeExpressionVisitor(activator);
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
        /// <param name="executionResult">The execution result.</param>
        protected virtual void AttachEntitiesToDataContext(object executionResult)
        {
            if (executionResult == null)
            {
                return;
            }

            var enumerableItemType = executionResult.GetType().TryGetEnumerableItemType();
            if (enumerableItemType != null && this.IsAttachableType(enumerableItemType))
            {
                foreach (var entity in (IEnumerable<object>)executionResult)
                {
                    if (this.IsAttachable(entity))
                    {
                        this.DataContext.AttachEntity(entity);
                    }
                }
            }
            else
            {
                var entity = executionResult;
                if (this.IsAttachable(entity))
                {
                    this.DataContext.AttachEntity(entity);
                }
            }
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