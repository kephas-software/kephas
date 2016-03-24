// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for the application root object.
    /// </summary>
    public abstract class ApplicationBase : IApplication
    {
        /// <summary>
        /// The application bootstrapper.
        /// </summary>
        private readonly IAppBootstrapper appBootstrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase"/> class.
        /// </summary>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appBootstrapper">The application bootstrapper.</param>
        protected ApplicationBase(string appId, IAppBootstrapper appBootstrapper)
        {
            Contract.Requires(!string.IsNullOrEmpty(appId));
            Contract.Requires(appBootstrapper != null);

            this.appBootstrapper = appBootstrapper;
            this.AppId = appId;
        }

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string AppId { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<IApplication> Logger { get; set; }

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual async Task StartAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.appBootstrapper.StartAsync(appContext, cancellationToken).WithServerThreadingContext();

                        cancellationToken.ThrowIfCancellationRequested();
                    },
                    this.Logger).WithServerThreadingContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Error(Strings.Application_StartCanceled_Exception, DateTimeOffset.Now);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.Application_StartFaulted_Exception, DateTimeOffset.Now);
                throw;
            }
        }
    }
}