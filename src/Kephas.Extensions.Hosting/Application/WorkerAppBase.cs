// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerAppBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the worker application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A worker application base.
    /// </summary>
    public abstract class WorkerAppBase : AppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerAppBase"/> class.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        /// <param name="appLifetimeTokenSource">Optional. The application lifetime token source.</param>
        protected WorkerAppBase(IAmbientServices? ambientServices = null, CancellationTokenSource? appLifetimeTokenSource = null)
            : base(ambientServices, appLifetimeTokenSource)
        {
        }

        /// <summary>
        /// Gets the host builder.
        /// </summary>
        /// <value>
        /// The host builder.
        /// </value>
        protected IHostBuilder HostBuilder { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        protected IHost Host { get; private set; }

        /// <summary>
        /// Bootstraps the application asynchronously.
        /// </summary>
        /// <param name="rawAppArgs">Optional. The application arguments.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override Task<(IAppContext? appContext, AppShutdownInstruction instruction)> BootstrapAsync(
            string[]? rawAppArgs = null,
            CancellationToken cancellationToken = default)
        {
            this.HostBuilder = this.CreateHostBuilder(rawAppArgs);

            this.HostBuilder
                .UseServiceProviderFactory(new CompositionServiceProviderFactory(this.AmbientServices));

            this.PreConfigureWorker(this.HostBuilder)
                .ConfigureServices(services =>
                {
                    this.AddBackgroundWorker(services);

                    this.AmbientServices
                        .WithServiceCollection(services)
                        .ConfigureExtensionsLogging();
                });

            this.ConfigureWorker(this.HostBuilder)
                .ConfigureServices(services =>
                {
                    this.BuildWorkerServicesContainer(this.AmbientServices);
                });

            this.PostConfigureWorker(this.HostBuilder);

            return base.BootstrapAsync(rawAppArgs, cancellationToken);
        }

        /// <summary>
        /// Initializes the application manager asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        protected override Task<IAppContext> InitializeAppManagerAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            appContext.InitializeAppManagerAsync(token => base.InitializeAppManagerAsync(appContext, token));
            return Task.FromResult(appContext);
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        protected sealed override void BuildServicesContainer(IAmbientServices ambientServices)
        {
            this.Host = this.HostBuilder.Build();
        }

        /// <summary>
        /// Configures the ambient services asynchronously for this worker.
        /// </summary>
        /// <remarks>
        /// Use this method instead of <see cref="BuildServicesContainer"/>
        /// for configuring the worker ambient services.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected abstract void BuildWorkerServicesContainer(IAmbientServices ambientServices);

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="rawAppArgs">The application arguments.</param>
        /// <returns>
        /// The new host builder.
        /// </returns>
        protected virtual IHostBuilder CreateHostBuilder(string[]? rawAppArgs)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(rawAppArgs);
        }

        /// <summary>
        /// Configures the worker before adding the background worker.
        ///  Here is the place where logging should be initialized.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>
        /// The provided <see cref="IHostBuilder"/>.
        /// </returns>
        protected virtual IHostBuilder PreConfigureWorker(IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }

        /// <summary>
        /// Configures the worker. Here is the place where the Windows service or the Linux daemon
        /// should be initialized.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>
        /// The provided <see cref="IHostBuilder"/>.
        /// </returns>
        protected virtual IHostBuilder ConfigureWorker(IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }

        /// <summary>
        /// Configures the worker after configuring the ambient services.
        /// Here is the place to do whatever initialization is necessary before actually
        /// going into the bootstrapping procedure.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>
        /// The provided <see cref="IHostBuilder"/>.
        /// </returns>
        protected virtual IHostBuilder PostConfigureWorker(IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }

        /// <summary>
        /// Adds the background worker as a hosted service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <example>
        /// .
        /// </example>
        protected virtual void AddBackgroundWorker(IServiceCollection services)
        {
            services.AddHostedService<BackgroundWorker>();
        }
    }
}
