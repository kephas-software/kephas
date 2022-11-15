﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingApplicationLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scheduling application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A scheduling application lifecycle behavior.
    /// </summary>
    [OverridePriority(Priority.High - 100)]
    public class SchedulingApplicationLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingApplicationLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public SchedulingApplicationLifecycleBehavior(IScheduler scheduler)
        {
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task<IOperationResult> AfterAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            await ServiceHelper.InitializeAsync(this.scheduler, cancellationToken: cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> BeforeAppFinalizeAsync(CancellationToken cancellationToken = default)
        {
            await ServiceHelper.FinalizeAsync(this.scheduler, cancellationToken: cancellationToken).PreserveThreadContext();
            return true.ToOperationResult();
        }
    }
}