// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveChangesInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Base interface for save change operations.
/// </summary>
public interface ISaveChangesOperation : IRepositoryOperation<int>
{
    /// <summary>
    /// Gets the repository.
    /// </summary>
    new IWriteRepository Repository { get; }

    /// <summary>
    /// Gets the repository.
    /// </summary>
    IReadRepository IRepositoryOperation.Repository => Repository;
    
    /// <summary>
    /// Gets the entity context.
    /// </summary>
    IEntityContext EntityContext { get; }
}

/// <summary>
/// The execute operation arguments.
/// </summary>
/// <param name="Repository">The repository.</param>
/// <param name="EntityContext">The entity context being used for saving changes.</param>
public record SaveChangesOperation(IWriteRepository Repository, IEntityContext EntityContext) : ISaveChangesOperation;

/// <summary>
/// Interceptor being invoked when the changes are persisted for an entity context.
/// </summary>
public interface ISaveChangesInterceptor : IRepositoryOperationInterceptor<ISaveChangesOperation, int>
{
    /// <summary>
    /// Invoked when saving the changes is executed with the provided arguments.
    /// </summary>
    /// <param name="operation">The intercepted operation.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    Task<int> IRepositoryOperationInterceptor<ISaveChangesOperation, int>.HandleAsync(
        ISaveChangesOperation operation,
        InterceptNextDelegate<int> next,
        CancellationToken cancellationToken) =>
        HandleAsync(operation.Repository, operation.EntityContext, () => next(), cancellationToken);

    /// <summary>
    /// Invoked when saving the changes is executed with the provided arguments.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="entityContext">The affected entity context.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext, Func<Task<int>> next, CancellationToken cancellationToken);    
}