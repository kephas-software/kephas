// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;
    using Kephas.Data.Linq;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IDataContext"/>.
    /// </summary>
    public static class DataContextExtensions
    {
        /// <summary>
        /// Creates the command with the provided type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command to be created.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        public static TCommand CreateCommand<TCommand>(this IDataContext dataContext)
            where TCommand : IDataCommand
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            return (TCommand)dataContext.CreateCommand(typeof(TCommand));
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity to create.</param>
        /// <param name="operationContext">Context for the create entity operation (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static Task<object> CreateEntityAsync(
            this IDataContext dataContext,
            Type entityType,
            ICreateEntityContext operationContext = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entityType, nameof(entityType));

            if (operationContext == null)
            {
                operationContext = new CreateEntityContext(dataContext, entityType);
            }

            return CreateEntityCoreAsync(dataContext, operationContext, cancellationToken);
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the entity to create.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="operationContext">Context for the create entity operation (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static async Task<T> CreateEntityAsync<T>(
            this IDataContext dataContext,
            ICreateEntityContext operationContext = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            if (operationContext == null)
            {
                operationContext = new CreateEntityContext<T>(dataContext);
            }

            return (T)await CreateEntityCoreAsync(dataContext, operationContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="id">The entity ID.</param>
        /// <param name="throwIfNotFound">If <c>true</c> and an entity with the provided ID is not found, an exception occurs (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static Task<object> FindAsync(
            this IDataContext dataContext,
            Type entityType,
            Id id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entityType, nameof(entityType));

            var findContext = new FindContext(dataContext, entityType, id, throwIfNotFound);
            return FindCoreAsync(dataContext, findContext, cancellationToken);
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">Context for the find operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static Task<object> FindAsync(
            this IDataContext dataContext,
            IFindContext findContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(findContext, nameof(findContext));

            return FindCoreAsync(dataContext, findContext, cancellationToken);
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">Context for the find operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataContext dataContext,
            IFindContext findContext,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(findContext, nameof(findContext));

            return (T)await FindCoreAsync(dataContext, findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="id">The entity ID.</param>
        /// <param name="throwIfNotFound">If <c>true</c> and an entity with the provided ID is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataContext dataContext,
            Id id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            var findContext = new FindContext<T>(dataContext, id, throwIfNotFound);
            return (T)await FindCoreAsync(dataContext, findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="throwIfNotFound"><c>true</c> to throw if not found (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindOneAsync<T>(
            this IDataContext dataContext, 
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(criteria, nameof(criteria));

            var queryContext = new QueryOperationContext(dataContext);
            var query = dataContext.Query<T>(queryContext).Where(criteria).Take(2);
            var result = await query.ToListAsync(cancellationToken).PreserveThreadContext();
            if (result.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataContext_FindOneAsync_AmbiguousMatch_Exception, criteria));
            }

            if (result.Count == 0 && throwIfNotFound)
            {
                throw new NotFoundDataException(string.Format(Strings.DataContext_FindOneAsync_NotFound_Exception, criteria));
            }

            return result.Count == 0 ? default(T) : result[0];
        }

        /// <summary>
        /// Persists the changes in the dataContext asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the persist result.
        /// </returns>
        public static async Task<IDataCommandResult> PersistChangesAsync(
            this IDataContext dataContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            var command = (IPersistChangesCommand)dataContext.CreateCommand(typeof(IPersistChangesCommand));
            var persistContext = new PersistChangesContext(dataContext);
            var result = await command.ExecuteAsync(persistContext, cancellationToken).PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Discards the changes in the data context.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <returns>
        /// The result of discarding the changes.
        /// </returns>
        public static IDataCommandResult DiscardChanges(this IDataContext dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            var command = (IDiscardChangesCommand)dataContext.CreateCommand(typeof(IDiscardChangesCommand));
            var persistContext = new DataOperationContext(dataContext);
            var result = command.Execute(persistContext);
            return result;
        }

        /// <summary>
        /// Marks the provided entity for deletion in the provided context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entity">The entity.</param>
        public static void DeleteEntity<T>(this IDataContext dataContext, T entity)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entity, nameof(entity));

            var command = (IDeleteEntityCommand)dataContext.CreateCommand(typeof(IDeleteEntityCommand));
            var deleteContext = new DeleteEntityContext(dataContext, entity);
            command.Execute(deleteContext);
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="operationContext">Context for the create entity operation (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        private static async Task<object> CreateEntityCoreAsync(
            this IDataContext dataContext,
            ICreateEntityContext operationContext,
            CancellationToken cancellationToken)
        {
            var command = (ICreateEntityCommand)dataContext.CreateCommand(typeof(ICreateEntityCommand));
            var result = await command.ExecuteAsync(operationContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Searches for the entity with the ID provided in the find context and returns it asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        private static async Task<object> FindCoreAsync(
            this IDataContext dataContext,
            IFindContext findContext,
            CancellationToken cancellationToken)
        {
            var command = (IFindCommand)dataContext.CreateCommand(typeof(IFindCommand));
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }
    }
}