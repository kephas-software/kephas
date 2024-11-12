// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExecuteInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.StoredProcedures;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Base interface for execute operations.
/// </summary>
/// <typeparam name="TSpArgs">The type of stored procedure arguments.</typeparam>
public interface IExecuteOperation<out TSpArgs> : IRepositoryOperation<object?>
    where TSpArgs : ISpArgs
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
    /// The stored procedure arguments.
    /// </summary>
    TSpArgs SpArgs { get; }
}

/// <summary>
/// The execute operation arguments.
/// </summary>
/// <param name="Repository">The repository.</param>
/// <param name="SpArgs">The stored procedure arguments.</param>
/// <typeparam name="TSpArgs">The type of the stored procedure arguments.</typeparam>
public record ExecuteOperation<TSpArgs>(IWriteRepository Repository, TSpArgs SpArgs) : IExecuteOperation<TSpArgs> where TSpArgs : ISpArgs;

/// <summary>
/// Interceptor for executing a stored procedure.
/// </summary>
/// <typeparam name="TSpArgs">The type of the stoed procedure arguments.</typeparam>
public interface IExecuteInterceptor<in TSpArgs> : IRepositoryOperationInterceptor<IExecuteOperation<TSpArgs>, object?>
    where TSpArgs : ISpArgs
{
    /// <summary>
    /// Invoked when the stored procedure is executed with the provided arguments.
    /// </summary>
    /// <param name="operation">The intercepted operation.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    async Task<object?> IRepositoryOperationInterceptor<IExecuteOperation<TSpArgs>, object?>.HandleAsync(
        IExecuteOperation<TSpArgs> operation,
        InterceptNextDelegate<object?> next,
        CancellationToken cancellationToken)
    {
        await HandleAsync(operation.Repository, operation.SpArgs, () => next(), cancellationToken);
        return null;
    }

    /// <summary>
    /// Invoked before the stored procedure is executed with the provided arguments.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="args">The stored procedure arguments.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    Task HandleAsync(IWriteRepository repository, TSpArgs args, Func<Task> next, CancellationToken cancellationToken);    
}
    