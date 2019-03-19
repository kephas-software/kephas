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
    using System.Threading;
    using System.Threading.Tasks;

    using global::Quartz.Logging;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Scheduling.Quartz.Logging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application lifecycle behavior initializing the Quartz infrastructure.
    /// </summary>
    public class QuartzAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public QuartzAppLifecycleBehavior(ILogManager logManager)
        {
            Requires.NotNull(logManager, nameof(logManager));

            this.logManager = logManager;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            LogProvider.SetCurrentLogProvider(new QuartzLogProvider(this.logManager));

            return TaskHelper.CompletedTask;
        }
    }
}