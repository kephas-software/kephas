// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Extensions.Hosting.Application;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The main loop service of an ASP.NET Core application.
    /// </summary>
    [Override]
    [ProcessingPriority(Priority.AboveNormal - 100)]
    public class AspNetMainLoop : IAppMainLoop
    {
        private readonly IEventSubscription? shutdownSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetMainLoop"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="appLifetime">Optional. The application lifetime.</param>
        /// <param name="logger">Optional. The logger.</param>
        public AspNetMainLoop(
            IEventHub eventHub,
            IAppArgs appArgs,
            IHostApplicationLifetime? appLifetime = null,
            ILogger<AspNetMainLoop>? logger = null)
        {
            this.AppArgs = appArgs;
            this.AppLifetime = appLifetime;
            this.Logger = logger;
            this.shutdownSubscription = eventHub.Subscribe<ShutdownSignal>(this.HandleShutdownSignal);
        }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        protected IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets the application lifetime.
        /// </summary>
        protected IHostApplicationLifetime? AppLifetime { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger<AspNetMainLoop>? Logger { get; }

        /// <summary>
        /// Executes the application's main loop asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public Task<MainLoopResult> Main(CancellationToken cancellationToken)
        {
            return Task.FromResult(new MainLoopResult(new OperationResult { OperationState = OperationState.InProgress }, AppShutdownInstruction.Ignore));
        }

        /// <summary>Handles the shutdown signal.</summary>
        /// <param name="signal">The shutdown signal.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleShutdownSignal(ShutdownSignal signal, IContext? context)
        {
            try
            {
                this.AppLifetime!.StopApplication();
                this.shutdownSubscription?.Dispose();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while stopping the application");
            }
        }
    }
}