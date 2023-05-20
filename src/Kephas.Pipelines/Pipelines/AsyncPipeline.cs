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
/// Default implementation of a <see cref="IAsyncPipeline{TTarget,TOperationArgs,TResult}"/>.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TOperationArgs">The operation arguments type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[OverridePriority(Priority.Low)]
public class AsyncPipeline<TTarget, TOperationArgs, TResult> : IAsyncPipeline<TTarget, TOperationArgs, TResult>
{
    private readonly IExportFactory<PipelineContext> contextFactory;
    private readonly ILazyEnumerable<IAsyncPipelineBehavior, PipelineBehaviorMetadata>? behaviors;
    private IReadOnlyList<IAsyncPipelineBehavior<TTarget, TOperationArgs, TResult>>? cachedPipelineBehaviors;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPipeline{TTarget,TOperationArgs,TResult}"/> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="behaviors">Optional. The behaviors.</param>
    /// <param name="logger">Optional. The logger.</param>
    public AsyncPipeline(
        IExportFactory<PipelineContext> contextFactory,
        ILazyEnumerable<IAsyncPipelineBehavior, PipelineBehaviorMetadata>? behaviors = null,
        ILogger<AsyncPipeline<TTarget, TOperationArgs, TResult>>? logger = null)
    {
        this.contextFactory = contextFactory;
        this.behaviors = behaviors;
        this.Logger = logger;
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger<AsyncPipeline<TTarget, TOperationArgs, TResult>>? Logger { get; }


    /// <summary>
    /// Processes the pipeline, invoking the behaviors in their priority order.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="args">The operation arguments.</param>
    /// <param name="context">An optional context for the operation. If not context is provided, one will be created for the scope of the operation.</param>
    /// <param name="operation">The operation to be executed.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the result.</returns>
    public Task<TResult> ProcessAsync(
        TTarget target,
        TOperationArgs args,
        IContext? context,
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default)
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
            Func<Task<TResult>>? next = null;
            next = async () =>
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
                        ? await behavior.InvokeAsync(ToFuncOfTaskOfObject(next!), target, args, context, cancellationToken).PreserveThreadContext()
                        : throw new NullReferenceException(PipelinesStrings.Pipeline_ProcessAsync_NullBehavior_Exception)
                    : await operation().PreserveThreadContext();

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
    protected virtual IReadOnlyList<IAsyncPipelineBehavior<TTarget, TOperationArgs, TResult>>? GetPipelineBehaviors() =>
        this.cachedPipelineBehaviors ??= this.behaviors?
            .SelectServices(b =>
                (b.Metadata.TargetType?.IsAssignableFrom(typeof(TTarget)) ?? true) &&                       // covariant
                (b.Metadata.OperationArgsType?.IsAssignableFrom(typeof(TOperationArgs)) ?? true) &&         // covariant
                (b.Metadata.ResultType is null || typeof(TResult).IsAssignableFrom(b.Metadata.ResultType)))   // contravariant
            .Cast<IAsyncPipelineBehavior<TTarget, TOperationArgs, TResult>>()
            .ToList();

    private static Func<Task<object?>> ToFuncOfTaskOfObject(Func<Task<TResult>> func)
    {
        return async () =>
        {
            var result = await func().PreserveThreadContext();
            return result;
        };
    }
}