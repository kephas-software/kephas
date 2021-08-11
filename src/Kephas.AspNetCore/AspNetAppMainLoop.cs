// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetAppMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net application termination awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// An ASP net application shutdown awaiter.
    /// </summary>
    [OverridePriority(Priority.High)]
    public class AspNetAppMainLoop : Loggable, IAppMainLoop
    {
        private readonly IHostApplicationLifetime appLifetime;
        private IEventSubscription shutdownSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetAppMainLoop"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public AspNetAppMainLoop(IEventHub eventHub, IHostApplicationLifetime appLifetime, ILogManager? logManager = null)
            : base(logManager)
        {
            this.appLifetime = appLifetime;
            this.shutdownSubscription = eventHub.Subscribe<ShutdownSignal>((e, ctx) => this.HandleShutdownSignal());
        }

        /// <summary>
        /// Waits for the application termination asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the termination result.
        /// </returns>
        public Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<(IOperationResult result, AppShutdownInstruction instruction)>((new OperationResult { OperationState = OperationState.InProgress }, AppShutdownInstruction.Ignore));
        }

        /// <summary>
        /// Handles the shutdown signal.
        /// </summary>
        protected virtual void HandleShutdownSignal()
        {
            try
            {
                this.appLifetime.StopApplication();
                this.shutdownSubscription.Dispose();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while stopping the application");
            }
        }
    }
}
