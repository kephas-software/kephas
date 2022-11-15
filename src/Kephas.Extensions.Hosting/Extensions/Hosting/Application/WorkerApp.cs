// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the worker application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting.Application
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services.Builder;
    using Kephas.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A worker application.
    /// </summary>
    public class WorkerApp : AppBase
    {
        private CancellationTokenSource? stoppingTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerApp"/> class.
        /// </summary>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="hostConfig">Optional. The host configuration.</param>
        /// <param name="servicesConfig">Optional. The services configuration.</param>
        protected WorkerApp(
            IAppArgs? appArgs = null,
            Action<IHostBuilder>? hostConfig = null,
            Action<IAppServiceCollectionBuilder>? servicesConfig = null)
            : this(CreateBuilder(hostConfig, appArgs), servicesConfig, appArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerApp"/> class.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <param name="servicesConfig">Optional. The services configuration.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected WorkerApp(
            IHostBuilder builder,
            Action<IAppServiceCollectionBuilder>? servicesConfig = null,
            IAppArgs? appArgs = null)
            : base(appArgs)
        {
            this.HostBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
            this.ServicesConfiguration = b =>
            {
                servicesConfig?.Invoke(b);

                if (this.AppArgs.RunAsService)
                {
                    this.HostBuilder.UseWindowsService();
                    this.HostBuilder.UseSystemd();
                }

                this.HostBuilder.ConfigureServices(this.AddBackgroundWorker);
            };
        }

        /// <summary>
        /// Gets the host builder.
        /// </summary>
        /// <value>
        /// The host builder.
        /// </value>
        protected IHostBuilder HostBuilder { get; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        protected IHost? Host { get; private set; }

        /// <summary>
        /// The main loop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task yielding the <see cref="MainLoopResult"/>.</returns>
        protected override Task<MainLoopResult> Main(CancellationToken cancellationToken)
        {
            return RunHostAsync(this.Host!, this.stoppingTokenSource!.Token);
        }

        /// <summary>
        /// This is the last step in the app's configuration, when all the services are set up
        /// and the container is built. For inheritors, this is the last place where services can
        /// be added before calling. By default, it only builds the Lite container, but any other container adapter
        /// can be used, like Autofac or System.Composition.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <returns>The service provider.</returns>
        protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
        {
            this.HostBuilder.ConfigureServices(servicesBuilder);

            this.Host ??= this.HostBuilder.Build();

            this.stoppingTokenSource = new CancellationTokenSource();
            var eventHub = this.ServiceProvider.GetRequiredService<IEventHub>();
            eventHub.Subscribe<ShutdownSignal>((_, _) => this.stoppingTokenSource.Cancel());

            return this.Host.Services;
        }

        /// <summary>
        /// Adds the background worker as a hosted service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddBackgroundWorker(IServiceCollection services)
        {
            services.AddHostedService(svc => new BackgroundWorker(this.RunBackgroundTaskAsync));
        }

        /// <summary>
        /// Runs the background task asynchronously.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token used for stopping the process.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual async Task RunBackgroundTaskAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(50, stoppingToken);
                }
                catch (Exception ex)
                {
                    this.Log(LogLevel.Warning, ex);
                }
            }
        }

        private static IHostBuilder CreateBuilder(Action<IHostBuilder>? builderOptions, IAppArgs? appArgs)
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(appArgs?.ToCommandArgs().ToArray() ?? Array.Empty<string>());
            builderOptions?.Invoke(builder);
            return builder;
        }

        private static async Task<MainLoopResult> RunHostAsync(IHost host, CancellationToken stoppingToken)
        {
            var opResult = new OperationResult<bool>(true);
            await host.RunAsync(stoppingToken).PreserveThreadContext();
            return new MainLoopResult(opResult.Complete(), AppShutdownInstruction.Shutdown);
        }
    }
}