// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppSetupService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default service for the <see cref="IAppSetupService"/> contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppSetupService : Loggable, IAppSetupService
    {
        private readonly IList<Lazy<IAppSetupHandler, AppServiceMetadata>> lazyHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppSetupService"/> class.
        /// </summary>
        /// <param name="lazyHandlers">The lazy setup handlers.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public DefaultAppSetupService(
            ICollection<Lazy<IAppSetupHandler, AppServiceMetadata>> lazyHandlers,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.lazyHandlers = lazyHandlers.Order().ToList();
        }

        /// <summary>
        /// Performs the application setup.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IOperationResult> SetupAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult { Elapsed = TimeSpan.Zero };
            foreach (var lazyHandler in this.lazyHandlers)
            {
                var wrappedResult = await Profiler.WithInfoStopwatchAsync(
                        () => this.SetupAsync(lazyHandler.Value, appContext, cancellationToken),
                        this.Logger,
                        lazyHandler.Metadata.ServiceName ?? lazyHandler.Metadata.ServiceInstanceType?.Name)
                    .PreserveThreadContext();
                result.Elapsed += wrappedResult.Elapsed;
                result.MergeMessages(wrappedResult.Value);
            }

            return result.Complete();
        }

        /// <summary>
        /// Performs the application setup.
        /// </summary>
        /// <param name="setupHandler">The setup handler.</param>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task<IOperationResult> SetupAsync(IAppSetupHandler setupHandler, IContext appContext, CancellationToken cancellationToken)
        {
            try
            {
                return await setupHandler.SetupAsync(appContext, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                return new OperationResult().Fail(ex);
            }
        }
    }
}