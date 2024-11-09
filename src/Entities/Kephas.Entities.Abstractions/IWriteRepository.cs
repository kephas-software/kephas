// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWriteRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Entities.StoredProcedures;

namespace Kephas.Entities;

/// <summary>
/// Repository for writing data.
/// </summary>
[ScopedService]
public interface IWriteRepository : IReadRepository
{
    /// <summary>
    /// Executes the stored procedure with the provided arguments.
    /// </summary>
    /// <param name="args">The stored procedure arguments. Their type is annotated with [<see cref="StoredProcedureAttribute"/>].</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task. Output parameters are retrieved in the provided <paramref name="args"/>.</returns>
    Task ExecuteAsync(ISpArgs args, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Attaches the entity, making the change tracker start tracking its changes.
    /// </summary>
    /// <param name="entity">The entity to attach.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    void Attach<T>(T entity)
        where T : class;
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    void Add<T>(T entity)
        where T : class;

    /// <summary>
    /// Update an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    void Update<T>(T entity)
        where T : class;

    /// <summary>
    /// Delete an existing entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    void Delete<T>(T entity)
        where T : class;

    /// <summary>
    /// Delete an existing entity from the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    void Delete<T>(Guid id)
        where T : class
    {
        if (!TryDelete<T>(id))
        {
            throw new EntityNotFoundException(typeof(T), id);
        }
    }

    /// <summary>
    /// Tries to delete an existing entity from the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns><c>true</c> if the entity was found, otherwise <c>false</c>.</returns>
    bool TryDelete<T>(Guid id)
        where T : class;

    /// <summary>
    /// Saves the changes asynchronously to the persistence layer.
    /// </summary>
    /// <remarks>Calling this method requires an enclosing transaction.</remarks>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
