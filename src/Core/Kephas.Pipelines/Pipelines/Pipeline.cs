// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pipeline.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.Resources;

namespace Kephas.Pipelines;

/// <summary>
/// Default implementation of a <see cref="IPipeline{TTarget,TOperationArgs,TResult}"/>.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[Priority(Priority.Low)]
public class Pipeline<TTarget, TOperationArgs, TResult> : IPipeline<TTarget, TOperationArgs, TResult>
{
    private readonly ILazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>? behaviors;
    private IReadOnlyList<IPipelineBehavior<TTarget, TOperationArgs, TResult>>? cachedPipelineBehaviors;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline{TTarget,TOperationArgs,TResult}"/> class.
    /// </summary>
    /// <param name="behaviors">Optional. The behaviors.</param>
    public Pipeline(ILazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>? behaviors = null)
    {
        this.behaviors = behaviors;
    }

    /// <summary>
    /// Processes the pipeline, invoking the behaviors in their priority order.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="args">The operation arguments.</param>
    /// <param name="operation">The operation to be executed.</param>
    /// <returns>The execution result.</returns>
    public TResult Process(
        TTarget target,
        TOperationArgs args,
        Func<TResult> operation)
    {
        _ = operation ?? throw new ArgumentNullException(nameof(operation));

        var pipelineBehaviors = GetPipelineBehaviors();
        if (pipelineBehaviors?.Count is not > 0)
        {
            var result = operation();
            return result;
        }

        using var enumerator = pipelineBehaviors.GetEnumerator();
        Func<TResult>? next = null;
        next = () =>
        {
            var hasBehavior = enumerator.MoveNext();
            var behavior = hasBehavior ? enumerator.Current : null;
            var result = hasBehavior
                ? behavior is not null
                    ? behavior.Invoke(ToFuncOfObject(next!), target, args)
                    : throw new NullReferenceException(PipelinesStrings.Pipeline_ProcessAsync_NullBehavior_Exception)
                : operation();

            return (TResult)result!;
        };

        return next();
    }

    /// <summary>
    /// Gets the pipeline behaviors.
    /// </summary>
    /// <returns></returns>
    protected virtual IReadOnlyList<IPipelineBehavior<TTarget, TOperationArgs, TResult>>? GetPipelineBehaviors() =>
        cachedPipelineBehaviors ??= behaviors?
            .SelectServices(m =>
                (m.TargetType?.IsAssignableFrom(typeof(TTarget)) ?? true) &&                       // covariant
                (m.OperationArgsType?.IsAssignableFrom(typeof(TOperationArgs)) ?? true) &&         // covariant
                (m.ResultType is null || typeof(TResult).IsAssignableFrom(m.ResultType)))   // contravariant
            .Cast<IPipelineBehavior<TTarget, TOperationArgs, TResult>>()
            .ToList();

    private static Func<object?> ToFuncOfObject(Func<TResult> func)
    {
        return () =>
        {
            var result = func();
            return result;
        };
    }
}
