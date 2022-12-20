// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application lifecycle behavior for setting up the application.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class SetupAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppSetupService appSetupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appSetupService">The application setup service.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public SetupAppLifecycleBehavior(
            IAppSetupService appSetupService,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appSetupService = appSetupService;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override Task<IOperationResult> AfterAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            return this.appSetupService.SetupAsync(appContext, cancellationToken);
        }
    }
}