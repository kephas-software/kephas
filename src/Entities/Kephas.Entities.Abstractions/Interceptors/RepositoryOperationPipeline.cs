// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryOperationPipeline.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// The default implementation of the <see cref="IRepositoryOperationPipeline"/>.
/// </summary>
/// <param name="factoryInterceptors">The enumeration of factory interceptors.</param>
public class RepositoryOperationPipeline(
    IFactoryEnumerable<IRepositoryOperationInterceptor, RepositoryOperationInterceptorMetadata> factoryInterceptors)
    : IRepositoryOperationPipeline
{
    /// <summary>
    /// Handles a repository operation asynchronously by invoking the interceptors and ultimately giving control to the .
    /// </summary>
    /// <param name="operationArgs">The operation arguments.</param>
    /// <param name="handler">The handler function.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>An awaitable task yielding the operation result.</returns>
    public async Task<TResult> HandleAsync<TResult>(IRepositoryOperation<TResult> operationArgs,
        Func<Task<TResult>> handler,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operationArgs);
        ArgumentNullException.ThrowIfNull(handler);

        var operationArgsType = operationArgs.GetType();
        var interceptors = factoryInterceptors
            .SelectServices(m => m.OperationType?.IsAssignableFrom(operationArgsType) is true)
            .OfType<IRepositoryOperationInterceptor<IRepositoryOperation<TResult>, TResult>>();
        using var interceptorEnumerator = interceptors.GetEnumerator();
        InterceptNextDelegate<TResult>? next = null;
        next = async () =>
        {
            if (interceptorEnumerator.MoveNext())
            {
                return await interceptorEnumerator.Current.HandleAsync(operationArgs, next!, cancellationToken);
            }

            return await handler();
        };

        return await next();
    }
}