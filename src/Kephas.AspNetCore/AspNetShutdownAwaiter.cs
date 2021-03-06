﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetShutdownAwaiter.cs" company="Kephas Software SRL">
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
    /// Shutdown awaiter for an ASP.NET Core application.
    /// </summary>
    [Override]
    [ProcessingPriority(Priority.AboveNormal - 100)]
    public class AspNetShutdownAwaiter : WorkerAppShutdownAwaiter
    {
        private readonly IEventSubscription? shutdownSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetShutdownAwaiter"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="appLifetime">Optional. The application lifetime.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public AspNetShutdownAwaiter(
            IEventHub eventHub,
            IAppArgs appArgs,
            IHostApplicationLifetime? appLifetime = null,
            ILogManager? logManager = null)
            : base(eventHub, null, logManager)
        {
            this.AppArgs = appArgs;
            this.AppLifetime = appLifetime;
            if (this.RunAsInteractive)
            {
                this.shutdownSubscription = eventHub.Subscribe<ShutdownSignal>((e, ctx) => this.HandleShutdownSignal());
            }
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
        /// Gets a value indicating whether the instance is run in interactive mode.
        /// </summary>
        protected virtual bool RunAsInteractive => !this.AppArgs.RunAsService;

        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public override async Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            return this.RunAsInteractive
                ? await base.WaitForShutdownSignalAsync(cancellationToken).PreserveThreadContext()
                : (new OperationResult { OperationState = OperationState.InProgress }, AppShutdownInstruction.Ignore);
        }

        /// <summary>Handles the shutdown signal.</summary>
        protected override void HandleShutdownSignal()
        {
            if (this.RunAsInteractive)
            {
                base.HandleShutdownSignal();
                return;
            }

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