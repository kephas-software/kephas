﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulerClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISchedulerClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Scheduling.Runtime;

namespace Kephas.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
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
            object jobInfo,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels the job asynchronously.
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
        public static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            Func<object?> operation,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(operation), options, cancellationToken);
        }

        public static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            Action operation,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(operation), options, cancellationToken);
        }

        public static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            Func<CancellationToken, Task<object?>> asyncOperation,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return EnqueueAsync(scheduler, new RuntimeFuncJobInfo(asyncOperation), options, cancellationToken);
        }

        private static Task<IJobResult> EnqueueAsync(
            this ISchedulerClient scheduler,
            RuntimeFuncJobInfo jobInfo,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            return scheduler.EnqueueAsync(jobInfo, null, null, options, cancellationToken);
        }
    }
}