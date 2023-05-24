// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundWorker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the background worker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A background worker.
    /// </summary>
    public class BackgroundWorker : BackgroundService
    {
        private readonly Func<CancellationToken, Task> executeFunc;
        private CancellationTokenRegistration stopRegistration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorker"/> class.
        /// </summary>
        /// <param name="executeFunc">The execute function.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public BackgroundWorker(Func<CancellationToken, Task> executeFunc, ILogManager? logManager = null)
        {
            executeFunc = executeFunc ?? throw new System.ArgumentNullException(nameof(executeFunc));

            this.executeFunc = executeFunc;
            this.Logger = (logManager ?? LoggingHelper.DefaultLogManager).GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" />
        /// starts. The implementation should return a task that represents the lifetime of the long
        /// running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when
        ///                             <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" />
        ///                             is called.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.
        /// </returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Logger.Info("Background worker started.");

            this.stopRegistration = stoppingToken.Register(() =>
            {
                this.Logger.Info("Background worker stopping...");
                this.stopRegistration.Dispose();
                this.Logger.Info("Background worker stopped.");
            });

            return this.executeFunc(stoppingToken);
        }
    }
}
