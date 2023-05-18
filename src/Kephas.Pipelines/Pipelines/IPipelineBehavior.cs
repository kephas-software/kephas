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
public interface IPipelineBehavior<TTarget, TOperationArgs, TResult> : IPipelineBehavior
{
    /// <summary>
    /// Invokes the behavior.
    /// </summary>
    /// <param name="next">The pipeline continuation delegate.</param>
    /// <param name="target">The target.</param>
    /// <param name="args">The operation arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<TResult> InvokeAsync(
        Func<Task<TResult>> next,
        TTarget target,
        TOperationArgs args,
        CancellationToken cancellationToken);
}