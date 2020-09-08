// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulerClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISchedulerClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Runtime;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Runtime;
    using Kephas.Services;
    using Kephas.Workflow;

    /// <summary>
    /// Interface for job scheduler client.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ISchedulerClient
    {
        /// <summary>
        /// Enqueues a new job and starts it asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="target">The target instance used by the job.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> EnqueueAsync(
            IJobInfo scheduledJob,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Enqueues a new job and starts it asynchronously.
        /// </summary>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="target">The target instance used by the job.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> EnqueueAsync(
            object scheduledJobId,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job instance.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelScheduledJobAsync(
            IJobInfo scheduledJob,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided scheduled job asynchronously.
        /// </summary>
        /// <param name="scheduledJobId">The ID of the scheduled job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelScheduledJobAsync(
            object scheduledJobId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels the running job asynchronously.
        /// </summary>
        /// <param name="runningJobId">The identifier of the running job.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelRunningJobAsync(
            object runningJobId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels the trigger asynchronously.
        /// </summary>
        /// <param name="triggerId">The trigger identifier.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelTriggerAsync(
            object triggerId,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="ISchedulerClient"/>.
    /// </summary>
    public static class SchedulerClientExtensions
    {
        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="friendlyName">The friendly name of the job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            Func<object?> operation,
            string? friendlyName = null,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, operation, friendlyName), options, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="friendlyName">The friendly name of the job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            Action operation,
            string? friendlyName = null,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, operation, friendlyName), options, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="asyncOperation">The asynchronous operation.</param>
        /// <param name="friendlyName">The friendly name of the job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            Func<CancellationToken, Task<object?>> asyncOperation,
            string? friendlyName = null,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(RuntimeTypeRegistry.Instance, asyncOperation, friendlyName), options, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        private static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            IJobInfo scheduledJob,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return scheduler.EnqueueAsync(scheduledJob, null, null, options, cancellationToken);
        }
    }
}
