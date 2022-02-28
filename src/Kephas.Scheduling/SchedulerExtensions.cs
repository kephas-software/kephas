// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulerExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling;

using Kephas.Dynamic;
using Kephas.Operations;
using Kephas.Runtime;
using Kephas.Scheduling.Reflection;
using Kephas.Scheduling.Runtime;
using Kephas.Scheduling.Triggers;
using Kephas.Workflow;

/// <summary>
/// Extension methods for <see cref="IScheduler"/>.
/// </summary>
public static class SchedulerExtensions
{
    /// <summary>
    /// Enqueues a new job and starts it asynchronously.
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="scheduledJob">The scheduled job.</param>
    /// <param name="target">Optional. The target instance used by the job.</param>
    /// <param name="arguments">Optional. The arguments.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. The trigger to start the job.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        IJobInfo scheduledJob,
        object? target = null,
        IDynamic? arguments = null,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        return EnqueueAsync(scheduler, scheduledJob, null, target, arguments, options, trigger, cancellationToken);
    }

    /// <summary>
    /// Enqueues a new job and starts it asynchronously.
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="scheduledJobId">The ID of the scheduled job.</param>
    /// <param name="target">Optional. The target instance used by the job.</param>
    /// <param name="arguments">Optional. The arguments.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. The trigger to start the job.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        object scheduledJobId,
        object? target = null,
        IDynamic? arguments = null,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        return EnqueueAsync(scheduler, null, scheduledJobId, target, arguments, options, trigger, cancellationToken);
    }

    /// <summary>
    /// Enqueues a new job asynchronously.
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="friendlyName">The friendly name of the job.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. The trigger to start the job.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        Func<object?> operation,
        string? friendlyName = null,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        return EnqueueAsync(
            scheduler,
            new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, operation, friendlyName),
            null,
            null,
            options,
            trigger,
            cancellationToken);
    }

    /// <summary>
    /// Enqueues a new job asynchronously.
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="friendlyName">The friendly name of the job.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. The trigger to start the job.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        Action operation,
        string? friendlyName = null,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        return EnqueueAsync(
            scheduler,
            new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, operation, friendlyName),
            null,
            null,
            options,
            trigger,
            cancellationToken);
    }

    /// <summary>
    /// Enqueues a new job asynchronously.
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="asyncOperation">The asynchronous operation.</param>
    /// <param name="friendlyName">The friendly name of the job.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. The trigger to start the job.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    public static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        Func<CancellationToken, Task<object?>> asyncOperation,
        string? friendlyName = null,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        return EnqueueAsync(
            scheduler,
            new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, asyncOperation, friendlyName),
            null,
            null,
            options,
            trigger,
            cancellationToken);
    }

    /// <summary>
    /// Enqueues a new job asynchronously.
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="scheduledJob">The scheduled job.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. The trigger.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    private static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        IJobInfo scheduledJob,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        return EnqueueAsync(scheduler, scheduledJob, null, null, null, options, trigger, cancellationToken);
    }

    /// <summary>
    /// Starts a new job asynchronously.
    /// <para>
    /// The job information provided may be either an ID, a qualified name, or a
    /// <see cref="IJobInfo"/>.
    /// </para>
    /// </summary>
    /// <param name="scheduler">The scheduler to act on.</param>
    /// <param name="scheduledJob">The scheduled job.</param>
    /// <param name="scheduledJobId">The ID of the scheduled job.</param>
    /// <param name="target">Target for the.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="options">Optional. Options for controlling the operation.</param>
    /// <param name="trigger">Optional. A trigger for the scheduled job.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An asynchronous result that yields the operation result.
    /// </returns>
    private static Task<IOperationResult<IJobInfo?>> EnqueueAsync(
        this IScheduler scheduler,
        IJobInfo? scheduledJob,
        object? scheduledJobId,
        object? target,
        IDynamic? arguments,
        Action<IActivityContext>? options = null,
        ITrigger? trigger = null,
        CancellationToken cancellationToken = default)
    {
        scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        var job = scheduledJob ?? scheduledJobId ?? throw new ArgumentNullException(nameof(scheduledJobId)); 

        return scheduler.EnqueueAsync(
            job,
            ctx => ctx
                .ScheduledJob(scheduledJob)
                .ScheduledJobId(scheduledJobId ?? scheduledJob?.Id)
                .Trigger(trigger)
                .Activity(target, arguments, options),
            cancellationToken);
    }
}