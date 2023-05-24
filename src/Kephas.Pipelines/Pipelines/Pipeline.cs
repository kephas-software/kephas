// --------------------------------------------------------------------------------------------------------------------
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
/// Default implementation of a <see cref="IPipeline{TTarget,TOperationArgs,TResult}"/>.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[OverridePriority(Priority.Low)]
public class Pipeline<TTarget, TOperationArgs, TResult> : IPipeline<TTarget, TOperationArgs, TResult>
{
    private readonly IExportFactory<PipelineContext> contextFactory;
    private readonly ILazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>? behaviors;
    private IReadOnlyList<IPipelineBehavior<TTarget, TOperationArgs, TResult>>? cachedPipelineBehaviors;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline{TTarget,TOperationArgs,TResult}"/> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="behaviors">Optional. The behaviors.</param>
    /// <param name="logger">Optional. The logger.</param>
    public Pipeline(
        IExportFactory<PipelineContext> contextFactory,
        ILazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>? behaviors = null,
        ILogger<Pipeline<TTarget, TOperationArgs, TResult>>? logger = null)
    {
        this.contextFactory = contextFactory;
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
    /// <param name="args">The operation arguments.</param>
    /// <param name="context">An optional context for the operation. If not context is provided, one will be created for the scope of the operation.</param>
    /// <param name="operation">The operation to be executed.</param>
    /// <returns>The execution result.</returns>
    public TResult Process(
        TTarget target,
        TOperationArgs args,
        IContext? context,
        Func<TResult> operation)
    {
        var pipelineBehaviors = this.GetPipelineBehaviors();
        var ownsContext = context is null;
        context ??= contextFactory.CreateExportedValue();
        try
        {
            if (pipelineBehaviors?.Count is not > 0)
            {
                if (this.Logger.IsDebugEnabled())
                {
                    this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokingOperation_Message, typeof(TTarget),
                        typeof(TOperationArgs), typeof(TResult));
                }

                var result = operation();

                if (this.Logger.IsDebugEnabled())
                {
                    this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokedOperation_Message, typeof(TTarget),
                        typeof(TOperationArgs), typeof(TResult));
                }

                return result;
            }

            using var enumerator = pipelineBehaviors.GetEnumerator();
            Func<TResult>? next = null;
            next = () =>
            {
                var hasBehavior = enumerator.MoveNext();
                var behavior = hasBehavior ? enumerator.Current : null;
                if (this.Logger.IsDebugEnabled())
                {
                    if (hasBehavior)
                    {
                        this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokingBehavior_Message,
                            behavior?.GetType(), typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                    }
                    else
                    {
                        this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokingOperation_Message,
                            typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                    }
                }

                var result = hasBehavior
                    ? behavior is not null
                        ? behavior.Invoke(ToFuncOfObject(next!), target, args, context)
                        : throw new NullReferenceException(PipelinesStrings.Pipeline_ProcessAsync_NullBehavior_Exception)
                    : operation();

                if (this.Logger.IsDebugEnabled())
                {
                    if (hasBehavior)
                    {
                        this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokedBehavior_Message,
                            behavior?.GetType(), typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                    }
                    else
                    {
                        this.Logger.Debug(PipelinesStrings.Pipeline_ProcessAsync_InvokedOperation_Message,
                            typeof(TTarget), typeof(TOperationArgs), typeof(TResult));
                    }
                }

                return (TResult)result!;
            };

            return next();
        }
        finally
        {
            if (ownsContext)
            {
                context.Dispose();
            }
        }
    }

    /// <summary>
    /// Gets the pipeline behaviors.
    /// </summary>
    /// <returns></returns>
    protected virtual IReadOnlyList<IPipelineBehavior<TTarget, TOperationArgs, TResult>>? GetPipelineBehaviors() =>
        this.cachedPipelineBehaviors ??= this.behaviors?
            .SelectServices(b =>
                (b.Metadata.TargetType?.IsAssignableFrom(typeof(TTarget)) ?? true) &&                       // covariant
                (b.Metadata.OperationArgsType?.IsAssignableFrom(typeof(TOperationArgs)) ?? true) &&         // covariant
                (b.Metadata.ResultType is null || typeof(TResult).IsAssignableFrom(b.Metadata.ResultType)))   // contravariant
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