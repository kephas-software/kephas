// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryOperationPipeline.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Service for the repository operation pipeline.
/// </summary>
[TransientService]
public interface IRepositoryOperationPipeline
{
    /// <summary>
    /// Handles a repository operation asynchronously by invoking the interceptors and ultimately giving control to the .
    /// </summary>
    /// <param name="operationArgs">The operation arguments.</param>
    /// <param name="handler">The handler function.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>An awaitable task yielding the operation result.</returns>
    Task<TResult> HandleAsync<TResult>(IRepositoryOperation<TResult> operationArgs,
        Func<Task<TResult>> handler,
        CancellationToken cancellationToken = default);
}