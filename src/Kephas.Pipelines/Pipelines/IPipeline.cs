// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipeline.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Pipelines;

/// <summary>
/// Contract for pipelines.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[AppServiceContract(AsOpenGeneric = true)]
public interface IPipeline<in TTarget, in TOperationArgs, TResult>
{
    /// <summary>
    /// Processes the pipeline, invoking the behaviors in their priority order.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="args">The operation arguments.</param>
    /// <param name="context">An optional context for the operation. If not context is provided, one will be created for the scope of the operation.</param>
    /// <param name="operation">The operation to be executed.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the result.</returns>
    Task<TResult> ProcessAsync(
        TTarget target,
        TOperationArgs args,
        IContext? context,
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}