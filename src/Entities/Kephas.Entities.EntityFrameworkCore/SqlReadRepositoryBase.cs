// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlReadRepositoryBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.Entities.Interceptors;
using Kephas.Entities.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Kephas.Entities;

/// <summary>
/// The base implementation of <see cref="IReadRepository"/> for EntityFrameworkCore.
/// </summary>
public abstract class SqlReadRepositoryBase(
    ISqlEntityContextSelector entityContextSelector,
    IRepositoryOperationPipeline operationPipeline)
    : IReadRepository
{
    /// <summary>
    /// Gets the <see cref="IEntityContext"/> used by this repository to access data.
    /// </summary>
    /// <param name="entityType">The entity type for which the entity context is required.</param>
    IEntityContext IReadRepository.GetEntityContext(Type entityType) =>
        (IEntityContext)GetDbContext(entityType);

    /// <summary>
    /// Gets a query over the entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="trackEntities">Indicates whether the returned entities should be tracked. Default is <c>false</c>.</param>
    /// <returns>A query over the entities of type <typeparamref name="T"/>.</returns>
    public virtual IQueryable<T> Query<T>(bool trackEntities = false) where T : class
    {
        var entityType = typeof(T);
        var queryOperation = new QueryOperation(this, (IEntityContext)GetDbContext(entityType), entityType);
        return (IQueryable<T>)operationPipeline.Handle(
            queryOperation,
            () =>
            {
                var dbSet = Set<T>();
                if (entityType.GetCustomAttribute<StoredProcedureAttribute>() is null)
                {
                    return trackEntities ? dbSet : dbSet.AsNoTracking();
                }

                var rawQuery = dbSet.FromSqlInterpolated(new QuerySqlString<T>(new EmptySpArgs()));
                return trackEntities ? rawQuery : rawQuery.AsNoTracking();
            });
    }

    /// <summary>
    /// Gets a query over the entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="args">The arguments for the stored procedure.</param>
    /// <param name="trackEntities">Indicates whether the returned entities should be tracked. Default is <c>false</c>.</param>
    /// <returns>A query over the entities of type <typeparamref name="T"/>.</returns>
    public virtual IQueryable<T> Query<T>(ISpArgs args, bool trackEntities = false) where T : class
    {
        var entityType = typeof(T);
        var queryOperation = new QueryOperation(this, (IEntityContext)GetDbContext(entityType), entityType);
        return (IQueryable<T>)operationPipeline.Handle(
            queryOperation,
            () =>
            {
                var rawQuery = Set<T>().FromSqlInterpolated(new QuerySqlString<T>(args));
                return trackEntities ? rawQuery : rawQuery.AsNoTracking();
            });
    }

    /// <summary>
    /// Tries to get the entity with the provided ID.
    /// </summary>
    /// <param name="id">The id of the requested entity.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The entity or, in case it is not found, <c>null</c>.</returns>
    public virtual T? TryGetById<T>(object id)
        where T : class, IIdentifiable =>
        Set<T>().Find(id);

    /// <summary>
    /// Tries to get the entity with the provided ID asynchronously.
    /// </summary>
    /// <param name="id">The id of the requested entity.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A task yielding the entity or, in case it is not found, an exception.</returns>
    public virtual Task<T?> TryGetByIdAsync<T>(object id, CancellationToken cancellationToken = default)
        where T : class, IIdentifiable =>
        Set<T>()
            .FindAsync([id], cancellationToken)
            .AsTask()!;

    /// <summary>
    /// Gets the <see cref="DbContext"/> used by this repository to access data.
    /// </summary>
    /// <param name="entityType">The entity type for which the entity context is required.</param>
    protected virtual DbContext GetDbContext(Type entityType) => (DbContext)entityContextSelector.GetEntityContext(entityType);

    /// <summary>
    /// Gets the database facade for the provided entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The database facade.</returns>
    protected virtual DatabaseFacade GetDatabase(Type entityType) => GetDbContext(entityType).Database;

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> serving the provided entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The <see cref="DbSet{TEntity}"/> serving serving the entity type.</returns>
    protected virtual DbSet<T> Set<T>() where T : class => GetDbContext(typeof(T)).Set<T>();
}