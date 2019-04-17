// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpaceExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IDataSpace"/>.
    /// </summary>
    public static class DataSpaceExtensions
    {
        /// <summary>
        /// Gets a query over the entity type for the given query operation context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="operationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public static IQueryable<T> Query<T>(this IDataSpace dataSpace, IQueryOperationContext operationContext = null)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            return dataSpace[typeof(T)].Query<T>(operationContext);
        }

        /// <summary>
        /// An IDataSpace extension method that gets The entity entry.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dataSpace">The dataSpace to act on.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity entry.
        /// </returns>
        public static IEntityEntry GetEntityEntry<T>(this IDataSpace dataSpace, T entity)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            return dataSpace[typeof(T)].GetEntityEntry(entity);
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="entityType">Type of the entity to create.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static Task<object> CreateAsync(
            this IDataSpace dataSpace,
            Type entityType,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(entityType, nameof(entityType));

            var dataContext = dataSpace[entityType];
            var operationContext = new CreateEntityContext(dataContext, entityType);

            return dataContext.CreateCoreAsync(operationContext, cancellationToken);
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="operationContext">Context for the create entity operation (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static Task<object> CreateAsync(
            this IDataSpace dataSpace,
            ICreateEntityContext operationContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(operationContext, nameof(operationContext));

            var dataContext = dataSpace[operationContext.EntityType];
            return dataContext.CreateCoreAsync(operationContext, cancellationToken);
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the entity to create.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static async Task<T> CreateAsync<T>(
            this IDataSpace dataSpace,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            var dataContext = dataSpace[typeof(T)];
            var operationContext = new CreateEntityContext<T>(dataContext);
            return (T)await dataContext.CreateCoreAsync(operationContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Creates asynchronously a new entity of the provided type and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the entity to create.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="operationContext">Context for the create entity operation (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the created entity.
        /// </returns>
        public static async Task<T> CreateAsync<T>(
            this IDataSpace dataSpace,
            ICreateEntityContext operationContext,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(operationContext, nameof(operationContext));

            var dataContext = dataSpace[typeof(T)];
            return (T)await dataContext.CreateCoreAsync(operationContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="id">The entity ID.</param>
        /// <param name="throwIfNotFound">If <c>true</c> and an entity with the provided ID is not found, an exception occurs (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static Task<object> FindAsync(
            this IDataSpace dataSpace,
            Type entityType,
            object id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(entityType, nameof(entityType));

            var dataContext = dataSpace[entityType];
            var findContext = new FindContext(dataContext, entityType, id, throwIfNotFound);
            return dataContext.FindCoreAsync(findContext, cancellationToken);
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="findContext">Context for the find operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static Task<object> FindAsync(
            this IDataSpace dataSpace,
            IFindContext findContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(findContext, nameof(findContext));

            var dataContext = dataSpace[findContext.EntityType];
            return dataContext.FindCoreAsync(findContext, cancellationToken);
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="findContext">Context for the find operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataSpace dataSpace,
            IFindContext findContext,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(findContext, nameof(findContext));

            var dataContext = dataSpace[typeof(T)];
            return (T)await dataContext.FindCoreAsync(findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the entity with the provided ID and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="id">The entity ID.</param>
        /// <param name="throwIfNotFound">If <c>true</c> and an entity with the provided ID is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindAsync<T>(
            this IDataSpace dataSpace,
            object id,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            var dataContext = dataSpace[typeof(T)];
            var findContext = new FindContext<T>(dataContext, id, throwIfNotFound);
            return (T)await dataContext.FindCoreAsync(findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<object> FindOneAsync(
            this IDataSpace dataSpace,
            IFindOneContext findContext,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(findContext, nameof(findContext));

            var dataContext = dataSpace[findContext.EntityType];
            return await dataContext.FindOneCoreAsync(findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindOneAsync<T>(
            this IDataSpace dataSpace,
            IFindOneContext findContext,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(findContext, nameof(findContext));

            var dataContext = dataSpace[typeof(T)];
            return (T)await dataContext.FindOneCoreAsync(findContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="throwIfNotFound"><c>true</c> to throw if not found (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        public static async Task<T> FindOneAsync<T>(
            this IDataSpace dataSpace,
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(criteria, nameof(criteria));

            var dataContext = dataSpace[typeof(T)];
            var findOneContext = new FindOneContext<T>(dataContext, criteria, throwIfNotFound);
            return (T)await dataContext.FindOneCoreAsync(findOneContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Persists the changes in the dataContext asynchronously.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the persist result.
        /// </returns>
        public static async Task<IEnumerable<IDataCommandResult>> PersistChangesAsync(
            this IDataSpace dataSpace,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            var results = new List<IDataCommandResult>();
            foreach (var dataContext in dataSpace)
            {
                results.Add(await dataContext.PersistChangesAsync(cancellationToken: cancellationToken).PreserveThreadContext());
            }

            return results;
        }

        /// <summary>
        /// Discards the changes in the data context.
        /// </summary>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <returns>
        /// The result of discarding the changes.
        /// </returns>
        public static IEnumerable<IDataCommandResult> DiscardChanges(this IDataSpace dataSpace)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));

            var results = new List<IDataCommandResult>();
            foreach (var dataContext in dataSpace)
            {
                results.Add(dataContext.DiscardChanges());
            }

            return results;
        }

        /// <summary>
        /// Marks the provided entity for deletion in the data context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="entities">The entities to delete.</param>
        public static void Delete<T>(this IDataSpace dataSpace, params T[] entities)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(entities, nameof(entities));

            var dataContext = dataSpace[typeof(T)];
            dataContext.Delete(entities);
        }

        /// <summary>
        /// Marks the provided entity for deletion in the data context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="dataSpace">The data space to act on.</param>
        /// <param name="entities">The entities to delete.</param>
        public static void Delete<T>(this IDataSpace dataSpace, IEnumerable<T> entities)
            where T : class
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(entities, nameof(entities));

            var dataContext = dataSpace[typeof(T)];
            dataContext.Delete(entities);
        }
    }
}