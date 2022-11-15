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
    public class SetupAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IAppSetupService appSetupService;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appSetupService">The application setup service.</param>
        /// <param name="appContext">The application context.</param>
        public SetupAppLifecycleBehavior(IAppSetupService appSetupService, IAppContext appContext)
        {
            this.appSetupService = appSetupService ?? throw new ArgumentNullException(nameof(appSetupService));
            this.appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> AfterAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            return this.appSetupService.SetupAsync(this.appContext, cancellationToken);
        }
    }
}