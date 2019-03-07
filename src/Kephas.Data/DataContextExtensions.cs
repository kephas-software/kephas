// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IDataContext"/>.
    /// </summary>
    public static class DataContextExtensions
    {
        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        public static IEntityInfo DetachEntity(this IDataContext dataContext, object entity)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entity, nameof(entity));

            var entityInfo = dataContext.GetEntityInfo(entity);
            return entityInfo == null ? null : dataContext.DetachEntity(entityInfo);
        }

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
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static Task<object> CreateEntityAsync(
            this IDataContext dataContext,
            Type entityType,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entityType, nameof(entityType));

            var operationContext = new CreateEntityContext(dataContext, entityType);

            return CreateEntityCoreAsync(dataContext, operationContext, cancellationToken);
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
        public static Task<object> CreateEntityAsync(
            this IDataContext dataContext,
            ICreateEntityContext operationContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(operationContext, nameof(operationContext));

            return CreateEntityCoreAsync(dataContext, operationContext, cancellationToken);
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the entity to create.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static async Task<T> CreateEntityAsync<T>(
            this IDataContext dataContext,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            var operationContext = new CreateEntityContext<T>(dataContext);
            return (T)await CreateEntityCoreAsync(dataContext, operationContext, cancellationToken).PreserveThreadContext();
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
            ICreateEntityContext operationContext,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(operationContext, nameof(operationContext));

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
            object id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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
            object id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            var findContext = new FindContext<T>(dataContext, id, throwIfNotFound);
            return (T)await FindCoreAsync(dataContext, findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<object> FindOneAsync(
            this IDataContext dataContext,
            IFindOneContext findContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(findContext, nameof(findContext));

            return await FindOneCoreAsync(dataContext, findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindOneAsync<T>(
            this IDataContext dataContext,
            IFindOneContext findContext,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(findContext, nameof(findContext));

            return (T)await FindOneCoreAsync(dataContext, findContext, cancellationToken).PreserveThreadContext();
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
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(criteria, nameof(criteria));

            var findOneContext = new FindOneContext<T>(dataContext, criteria, throwIfNotFound);
            return (T)await FindOneCoreAsync(dataContext, findOneContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Persists the changes in the dataContext asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="persistContext">The context for persisting changes (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the persist result.
        /// </returns>
        public static async Task<IDataCommandResult> PersistChangesAsync(
            this IDataContext dataContext,
            IPersistChangesContext persistContext = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));

            persistContext = persistContext ?? new PersistChangesContext(dataContext);
            if (persistContext.DataContext != dataContext)
            {
                throw new DataException(Strings.DataContext_MismatchedDataContextInCommand_Exception);
            }

            var command = (IPersistChangesCommand)dataContext.CreateCommand(typeof(IPersistChangesCommand));
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
            var persistContext = new DiscardChangesContext(dataContext);
            var result = command.Execute(persistContext);
            return result;
        }

        /// <summary>
        /// Marks the provided entity for deletion in the data context.
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
        /// Deletes the entities matching the provided criteria and returns the number of affected entities asynchronously.
        /// </summary>
        /// <remarks>
        /// The entities are physically removed from the database without invoking any behavior.
        /// </remarks>
        /// <param name="dataContext">The data context.</param>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the number of affected entities.
        /// </returns>
        public static async Task<long> BulkDeleteAsync(
            this IDataContext dataContext,
            IBulkDeleteContext bulkDeleteContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(bulkDeleteContext, nameof(bulkDeleteContext));

            return await BulkDeleteCoreAsync(dataContext, bulkDeleteContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Deletes the entities matching the provided criteria and returns the number of affected entities asynchronously.
        /// </summary>
        /// <remarks>
        /// The entities are physically removed from the database without invoking any behavior.
        /// </remarks>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The matching criteria for entities to delete.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw if not found.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the number of affected entities.
        /// </returns>
        public static Task<long> BulkDeleteAsync<T>(
            this IDataContext dataContext,
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(criteria, nameof(criteria));

            var bulkDeleteContext = new BulkDeleteContext<T>(dataContext, criteria, throwIfNotFound);
            return BulkDeleteCoreAsync(dataContext, bulkDeleteContext, cancellationToken);
        }

        /// <summary>
        /// Updates the entities matching the provided criteria and returns the number of affected entities asynchronously.
        /// </summary>
        /// <remarks>
        /// The entities are physically updated in the database without invoking any behavior.
        /// </remarks>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The matching criteria for entities to delete.</param>
        /// <param name="values">The values.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw if not found.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the number of affected entities.
        /// </returns>
        public static Task<long> BulkUpdateAsync<T>(
            this IDataContext dataContext,
            Expression<Func<T, bool>> criteria,
            object values,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(criteria, nameof(criteria));
            Requires.NotNull(values, nameof(values));

            var bulkUpdateContext = new BulkUpdateContext<T>(dataContext, criteria, values, throwIfNotFound);
            return BulkUpdateCoreAsync(dataContext, bulkUpdateContext, cancellationToken);
        }

        /// <summary>
        /// Updates the entities matching the provided criteria and returns the number of affected entities asynchronously.
        /// </summary>
        /// <remarks>
        /// The entities are physically updated in the database without invoking any behavior.
        /// </remarks>
        /// <param name="dataContext">The data context.</param>
        /// <param name="bulkUpdateContext">The bulk update context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the number of affected entities.
        /// </returns>
        public static async Task<long> BulkUpdateAsync(
            this IDataContext dataContext,
            IBulkUpdateContext bulkUpdateContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(bulkUpdateContext, nameof(bulkUpdateContext));

            return await BulkUpdateCoreAsync(dataContext, bulkUpdateContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Executes the provided command in the data context.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the command execution result.
        /// </returns>
        public static async Task<object> ExecuteAsync(this IDataContext dataContext, string commandText, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNullOrEmpty(commandText, nameof(commandText));

            var command = (IExecuteCommand)dataContext.CreateCommand(typeof(IExecuteCommand));
            var executeContext = new ExecuteContext(dataContext) { CommandText = commandText };

            var commandResult = await command.ExecuteAsync(executeContext, cancellationToken).PreserveThreadContext();
            return commandResult.Result;
        }

        /// <summary>
        /// Executes the provided command in the data context.
        /// </summary>
        /// <exception cref="DataException">Thrown when a Data error condition occurs.</exception>
        /// <param name="dataContext">The data context.</param>
        /// <param name="executeContext">Context for the execution.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the command execution result.
        /// </returns>
        public static async Task<object> ExecuteAsync(this IDataContext dataContext, IExecuteContext executeContext, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(executeContext, nameof(executeContext));

            if (executeContext.DataContext != dataContext)
            {
                throw new DataException(Strings.DataContext_MismatchedDataContextInCommand_Exception);
            }

            var command = (IExecuteCommand)dataContext.CreateCommand(typeof(IExecuteCommand));

            var commandResult = await command.ExecuteAsync(executeContext, cancellationToken).PreserveThreadContext();
            return commandResult.Result;
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
        internal static async Task<object> CreateEntityCoreAsync(
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
        internal static async Task<object> FindCoreAsync(
            this IDataContext dataContext,
            IFindContext findContext,
            CancellationToken cancellationToken)
        {
            var command = (IFindCommand)dataContext.CreateCommand(typeof(IFindCommand));
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Searches for the entity with the provided criteria in the find context and returns it asynchronously.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        internal static async Task<object> FindOneCoreAsync(
            this IDataContext dataContext,
            IFindOneContext findContext,
            CancellationToken cancellationToken)
        {
            var command = (IFindOneCommand)dataContext.CreateCommand(typeof(IFindOneCommand));
            var result = await command.ExecuteAsync(findContext, cancellationToken).PreserveThreadContext();
            return result.Entity;
        }

        /// <summary>
        /// Deletes the entities matching the provided criteria and returns the number of affected entities asynchronously.
        /// </summary>
        /// <remarks>
        /// The entities are physically removed from the database without invoking any behavior.
        /// </remarks>
        /// <param name="dataContext">The data context.</param>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the number of affected entities.
        /// </returns>
        internal static async Task<long> BulkDeleteCoreAsync(
            this IDataContext dataContext,
            IBulkDeleteContext bulkDeleteContext,
            CancellationToken cancellationToken)
        {
            var command = (IBulkDeleteCommand)dataContext.CreateCommand(typeof(IBulkDeleteCommand));
            var result = await command.ExecuteAsync(bulkDeleteContext, cancellationToken).PreserveThreadContext();

            return result.Count;
        }

        /// <summary>
        /// Updates the entities matching the provided criteria and returns the number of affected entities asynchronously.
        /// </summary>
        /// <remarks>
        /// The entities are physically updated in the database without invoking any behavior.
        /// </remarks>
        /// <param name="dataContext">The data context.</param>
        /// <param name="bulkUpdateContext">The bulk update context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the number of affected entities.
        /// </returns>
        internal static async Task<long> BulkUpdateCoreAsync(
            this IDataContext dataContext,
            IBulkUpdateContext bulkUpdateContext,
            CancellationToken cancellationToken)
        {
            var command = (IBulkUpdateCommand)dataContext.CreateCommand(typeof(IBulkUpdateCommand));
            var result = await command.ExecuteAsync(bulkUpdateContext, cancellationToken).PreserveThreadContext();

            return result.Count;
        }
    }
}