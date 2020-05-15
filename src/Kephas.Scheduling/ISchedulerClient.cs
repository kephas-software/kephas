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
        /// Enqueues a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a
        /// <see cref="IJobInfo"/>.
        /// </para>
        /// </summary>
        /// <param name="jobInfo">Information describing the job.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> EnqueueAsync(
            IJobInfo jobInfo,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a
        /// <see cref="IJobInfo"/>.
        /// </para>
        /// </summary>
        /// <param name="jobInfoId">The ID of the job information.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> EnqueueAsync(
            object jobInfoId,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided job information asynchronously.
        /// </summary>
        /// <param name="jobInfo">The job information instance.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelJobInfoAsync(
            IJobInfo jobInfo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels all running jobs and active triggers related to the provided job information asynchronously.
        /// </summary>
        /// <param name="jobInfoId">The ID of the job information.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelJobInfoAsync(
            object jobInfoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels the running job asynchronously.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IJobResult> CancelJobAsync(
            object jobId,
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
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(operation, friendlyName), options, cancellationToken);
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
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(operation, friendlyName), options, cancellationToken);
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
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(asyncOperation, friendlyName), options, cancellationToken);
        }

        /// <summary>
        /// Enqueues a new job asynchronously.
        /// </summary>
        /// <param name="scheduler">The scheduler to act on.</param>
        /// <param name="jobInfo">The job information.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        private static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            IJobInfo jobInfo,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return scheduler.EnqueueAsync(jobInfo, null, null, options, cancellationToken);
        }
    }
}
