// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerAppShutdownAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the worker application shutdown awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Services;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A service application shutdown awaiter.
    /// </summary>
    [Override]
    [OverridePriority(Priority.AboveNormal)]
    public class WorkerAppShutdownAwaiter : DefaultAppShutdownAwaiter, IInitializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerAppShutdownAwaiter"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="host">Optional. The host.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public WorkerAppShutdownAwaiter(IEventHub eventHub, IHost? host = null, ILogManager? logManager = null)
            : base(eventHub, logManager)
        {
            this.Host = host;
        }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        protected IContext? AppContext { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        protected IHost? Host { get; }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        public virtual void Initialize(IContext? context = null)
        {
            this.AppContext = context;
        }

        /// <summary>
        /// Executes the unattended asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override Task RunUnattendedAsync(CancellationToken cancellationToken)
        {
            return this.Host != null
                ? this.Host.RunAsync(cancellationToken)
                : base.RunUnattendedAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the attended asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override Task RunAttendedAsync(CancellationToken cancellationToken)
        {
            return this.Host != null
                ? this.Host.RunAsync(cancellationToken)
                : this.StartShellAsync(cancellationToken);
        }

        /// <summary>
        /// Starts the shell asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task StartShellAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
