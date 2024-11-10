// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq.Expressions;
using Kephas.Collections;
using Kephas.DependencyInjection;
using Kephas.Entities.StoredProcedures;

namespace Kephas.Entities;

/// <summary>
/// Base contract for repositories used in reading data.
/// </summary>
[ScopedService]
public interface IReadRepository
{
    /// <summary>
    /// Gets the <see cref="IEntityContext"/> used by this repository to access data.
    /// </summary>
    /// <param name="entityType">The entity type for which the data context is required.</param>
    protected internal IEntityContext GetEntityContext(Type entityType);

    /// <summary>
    /// Gets a query over the entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="trackEntities">Indicates whether the returned entities should be tracked. Default is <c>false</c>.</param>
    /// <returns>A query over the entities of type <typeparamref name="T"/>.</returns>
    IQueryable<T> Query<T>(bool trackEntities = false)
        where T : class;

    /// <summary>
    /// Gets a query over the entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="args">The arguments for the stored procedure.</param>
    /// <param name="trackEntities">Indicates whether the returned entities should be tracked. Default is <c>false</c>.</param>
    /// <returns>A query over the entities of type <typeparamref name="T"/>.</returns>
    IQueryable<T> Query<T>(ISpArgs args, bool trackEntities = false)
        where T : class;

    /// <summary>
    /// Gets the entity with the provided ID.
    /// </summary>
    /// <param name="id">The id of the requested entity.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The entity or, in case it is not found, an exception occurs.</returns>
    T GetById<T>(object id)
        where T : IIdentifiable =>
        TryGetById<T>(id) ?? throw new EntityNotFoundException(typeof(T), id);

    /// <summary>
    /// Tries to get the entity with the provided ID.
    /// </summary>
    /// <param name="id">The id of the requested entity.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The entity or, in case it is not found, <c>null</c>.</returns>
    T? TryGetById<T>(object id)
        where T : IIdentifiable;

    /// <summary>
    /// Tries to get the entity with the provided ID asynchronously.
    /// </summary>
    /// <param name="id">The id of the requested entity.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, an exception.</returns>
    Task<T?> TryGetByIdAsync<T>(object id, CancellationToken cancellationToken = default)
        where T : IIdentifiable;

    /// <summary>
    /// Gets the entity with the provided ID asynchronously.
    /// </summary>
    /// <param name="id">The id of the requested entity.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, an exception.</returns>
    async Task<T> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default)
        where T : IIdentifiable =>
        await TryGetByIdAsync<T>(id, cancellationToken)
        ?? throw new EntityNotFoundException(typeof(T), id);

    /// <summary>
    /// Tries to get the entity with the provided condition asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, <c>null</c>.</returns>
    Task<T?> TryGetAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        where T : class =>
        TryGetAsync(predicate, null, cancellationToken);

    /// <summary>
    /// Tries to get the entity with the provided condition asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="queryConfig">Optional. Additional query configuration. This can be used, for example, to include additional navigation properties.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, <c>null</c>.</returns>
    Task<T?> TryGetAsync<T>(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? queryConfig, CancellationToken cancellationToken = default)
        where T : class
    {
        var query = Query<T>(trackEntities: true);
        query = queryConfig?.Invoke(query) ?? query;

        return query
            .SingleOrDefaultAsync(predicate ?? throw new ArgumentNullException(nameof(predicate)), cancellationToken: cancellationToken)
            .AsTask();
    }

    /// <summary>
    /// Gets the entity with the provided condition asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, an exception.</returns>
    Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        where T : class =>
        GetAsync(predicate, null, cancellationToken);

    /// <summary>
    /// Gets the entity with the provided condition asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="queryConfig">Optional. Additional query configuration. This can be used, for example, to include additional navigation properties.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, an exception.</returns>
    async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? queryConfig, CancellationToken cancellationToken = default)
        where T : class =>
        await TryGetAsync(predicate, queryConfig, cancellationToken)
        ?? throw new EntityNotFoundException(typeof(T), null);
}
