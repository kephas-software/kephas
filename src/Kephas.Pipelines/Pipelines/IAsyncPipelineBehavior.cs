// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipelineBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Pipelines;

/// <summary>
/// Base interface for asynchronous pipeline behaviors.
/// </summary>
public interface IAsyncPipelineBehavior
{
}

/// <summary>
/// Contract for asynchronous pipeline behaviors.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[AppServiceContract(AllowMultiple = true, ContractType = typeof(IAsyncPipelineBehavior))]
public interface IAsyncPipelineBehavior<in TTarget, in TOperationArgs, out TResult> : IAsyncPipelineBehavior
{
    /// <summary>
    /// Invokes the behavior.
    /// </summary>
    /// <remarks>
    /// Make sure to return a result convertible to <typeparamref name="TResult"/>.
    /// Due to the fact that the <typeparamref name="TResult"/> must be contravariant
    /// so that generic pipeline behaviors may handle all kind of results.
    /// </remarks>
    /// <param name="next">The pipeline continuation delegate.</param>
    /// <param name="target">The target.</param>
    /// <param name="args">The operation arguments.</param>
    /// <param name="context">The operation context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task yielding the invocation result.</returns>
    Task<object?> InvokeAsync(
        Func<Task<object?>> next,
        TTarget target,
        TOperationArgs args,
        IContext context,
        CancellationToken cancellationToken);
}