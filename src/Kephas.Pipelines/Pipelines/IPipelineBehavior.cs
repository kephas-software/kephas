// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipelineBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Pipelines;

/// <summary>
/// Base interface for pipeline behaviors.
/// </summary>
public interface IPipelineBehavior
{
}

/// <summary>
/// Contract for pipeline behaviors.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[AppServiceContract(AllowMultiple = true, ContractType = typeof(IPipelineBehavior))]
public interface IPipelineBehavior<in TTarget, in TOperationArgs, out TResult> : IPipelineBehavior
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
    /// <returns>The invocation result.</returns>
    object? Invoke(
        Func<object?> next,
        TTarget target,
        TOperationArgs args,
        IContext context);
}