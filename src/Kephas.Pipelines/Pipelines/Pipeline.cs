﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pipeline.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Logging;
using Kephas.Resources;
using Kephas.Services;
using Kephas.Threading.Tasks;

namespace Kephas.Pipelines;

/// <summary>
/// Default implementation of a <see cref="IPipeline{TTarget,TContext,TResult}"/>.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[OverridePriority(Priority.Low)]
public class Pipeline<TTarget, TOperationArgs, TResult> : IPipeline<TTarget, TOperationArgs, TResult>
{
    private readonly ILazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>? behaviors;
    private IReadOnlyList<IPipelineBehavior<TTarget, TOperationArgs, TResult>>? cachedPipelineBehaviors;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline{TTarget,TOperationArgs,TResult}"/> class.
    /// </summary>
    /// <param name="behaviors">Optional. The behaviors.</param>
    /// <param name="logger">Optional. The logger.</param>
    public Pipeline(
        ILazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>? behaviors = null,
        ILogger<Pipeline<TTarget, TOperationArgs, TResult>>? logger = null)
    {
        this.behaviors = behaviors;
        this.Logger = logger;
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger<Pipeline<TTarget, TOperationArgs, TResult>>? Logger { get; }

    /// <summary>
    /// Processes the pipeline, invoking the behaviors in their priority order.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="args">The args.</param>
    /// <param name="operation">The operation to be executed.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the result.</returns>
    public Task<TResult> ProcessAsync(
        TTarget target,
        TOperationArgs args,
        Func<TTarget, TOperationArgs, CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        var pipelineBehaviors = this.GetPipelineBehaviors();
        if (pipelineBehaviors?.Count is not > 0)
        {
            if (this.Logger.IsDebugEnabled())
            {
                this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokingOperation_Message, typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
            }

            var result = operation.Invoke(target, args, cancellationToken);

            if (this.Logger.IsDebugEnabled())
            {
                this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokedOperation_Message, typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
            }

            return result;
        }

        using var enumerator = pipelineBehaviors.GetEnumerator();
        Func<Task<TResult>>? next = null;
        next = async () =>
        {
            var hasBehavior = enumerator.MoveNext();
            var behavior = hasBehavior ? enumerator.Current : null;
            if (this.Logger.IsDebugEnabled())
            {
                if (hasBehavior)
                {
                    this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokingBehavior_Message, behavior?.GetType(), typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                }
                else
                {
                    this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokingOperation_Message, typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                }
            }

            var result = hasBehavior
                ? behavior is not null
                    ? await behavior.InvokeAsync(next!, target, args, cancellationToken).PreserveThreadContext()
                    : throw new NullReferenceException(PipelinesStrings.Pipeline_ProcessAsync_NullBehavior_Exception)
                : await operation.Invoke(target, args, cancellationToken).PreserveThreadContext();

            if (this.Logger.IsDebugEnabled())
            {
                if (hasBehavior)
                {
                    this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokedBehavior_Message, behavior?.GetType(), typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                }
                else
                {
                    this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokedOperation_Message, typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                }
            }

            return result;
        };

        return next();
    }

    /// <summary>
    /// Gets the pipeline behaviors.
    /// </summary>
    /// <returns></returns>
    protected virtual IReadOnlyList<IPipelineBehavior<TTarget, TOperationArgs, TResult>>? GetPipelineBehaviors() =>
        this.cachedPipelineBehaviors ??= this.behaviors?
            .SelectServices(b =>
                b.Metadata.TargetType == typeof(TTarget) &&
                b.Metadata.OperationArgsType == typeof(TOperationArgs) &&
                b.Metadata.ResultType == typeof(TResult))
            .Cast<IPipelineBehavior<TTarget, TOperationArgs, TResult>>()
            .ToList();
}