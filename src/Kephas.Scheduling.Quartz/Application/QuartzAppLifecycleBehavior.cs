// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuartzAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quartz application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Quartz.Logging;
    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Quartz.Logging;

    /// <summary>
    /// Application lifecycle behavior initializing the Quartz infrastructure.
    /// </summary>
    public class QuartzAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public QuartzAppLifecycleBehavior(ILogManager logManager)
        {
            logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));

            this.logManager = logManager;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            LogProvider.SetCurrentLogProvider(new QuartzLogProvider(this.logManager));

            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}