// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryOperationPipeline.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.DependencyInjection;
using Kephas.Reflection;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// The default implementation of the <see cref="IRepositoryOperationPipeline"/>.
/// </summary>
/// <param name="factoryInterceptors">The enumeration of factory interceptors.</param>
public class RepositoryOperationPipeline(
    IFactoryEnumerable<IRepositoryInterceptor, RepositoryInterceptorMetadata> factoryInterceptors)
    : IRepositoryOperationPipeline
{
    private static readonly MethodInfo HandleAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ =>
        HandleAsync<IRepositoryOperation<int>, int>(null!, null!, null!, default));
    
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
            .Where(f => f.Metadata.OperationType?.IsAssignableFrom(operationArgsType) is true);
        using var interceptorEnumerator = interceptors.GetEnumerator();
        InterceptNextDelegate<TResult>? next = null;
        next = async () =>
        {
            if (interceptorEnumerator.MoveNext())
            {
                var interceptorFactory = interceptorEnumerator.Current;
                var handleAsync = HandleAsyncMethod.MakeGenericMethod(
                    interceptorFactory.Metadata.OperationType!,
                    interceptorFactory.Metadata.ResultType!);
                return await handleAsync.Call<Task<TResult>>(null, interceptorFactory.GetService(), operationArgs, next!, cancellationToken);
            }

            return await handler();
        };

        return await next();
    }
    
    private static Task<TResult> HandleAsync<TOperation, TResult>(IRepositoryInterceptor<TOperation, TResult> interceptor, TOperation operationArgs, InterceptNextDelegate<TResult> next, CancellationToken cancellationToken)
        where TOperation : IRepositoryOperation<TResult> =>
        interceptor.HandleAsync(operationArgs, next, cancellationToken);
}