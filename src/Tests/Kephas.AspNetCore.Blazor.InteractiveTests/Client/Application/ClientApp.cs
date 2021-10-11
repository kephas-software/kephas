// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client.Application
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Logging;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The client application.
    /// </summary>
    /// <seealso cref="Kephas.Application.AppBase" />
    public class ClientApp<TApp> : AppBase
        where TApp : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientApp{TApp}"/> class.
        /// </summary>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="containerBuilder">Optional. The container builder.</param>
        public ClientApp(
            IAppArgs? appArgs = null,
            Action<IAmbientServices>? containerBuilder = null)
            : base(new AmbientServices(), appLifetimeTokenSource: null, containerBuilder)
        {
            this.AppArgs = appArgs ?? new AppArgs();
        }

        /// <summary>
        /// Gets the host builder.
        /// </summary>
        /// <value>
        /// The host builder.
        /// </value>
        protected WebAssemblyHostBuilder? HostBuilder { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        protected WebAssemblyHost? Host { get; private set; }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        protected IAppArgs AppArgs { get; }

        /// <summary>
        /// Bootstraps the application asynchronously.
        /// </summary>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override Task<(IAppContext? appContext, AppShutdownInstruction instruction)> BootstrapAsync(
            IAppArgs? appArgs = null,
            CancellationToken cancellationToken = default)
        {
            this.HostBuilder = this.CreateHostBuilder(appArgs ?? this.AppArgs);

            this.HostBuilder
                .ConfigureContainer(new InjectionServiceProviderFactory(this.AmbientServices));

            this.PreConfigureWorker(
                this.HostBuilder,
                services =>
                {
                    services.AddSingleton<WebAssemblyHost>(_ => this.Host!);

                    this.AmbientServices
                        .WithServiceCollection(services)
                        .ConfigureExtensionsLogging();
                });

            this.ConfigureWorker(
                this.HostBuilder,
                services =>
                {
                    this.BuildWorkerServicesContainer(this.AmbientServices);
                });

            this.PostConfigureWorker(this.HostBuilder);

            return base.BootstrapAsync(appArgs, cancellationToken);
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// The new host builder.
        /// </returns>
        protected virtual WebAssemblyHostBuilder CreateHostBuilder(IAppArgs appArgs)
        {
            return WebAssemblyHostBuilder.CreateDefault(appArgs.ToCommandArgs().ToArray());
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
            // delay the initialization of the app manager until the host is started.
            return Task.FromResult(appContext);
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        protected sealed override void BuildServicesContainer(IAmbientServices ambientServices)
        {
            this.Host = this.HostBuilder!.Build();
        }

        /// <summary>
        /// Configures the ambient services asynchronously for this worker.
        /// </summary>
        /// <remarks>
        /// Use this method instead of <see cref="BuildServicesContainer"/>
        /// for configuring the worker ambient services.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected virtual void BuildWorkerServicesContainer(IAmbientServices ambientServices)
        {
            base.BuildServicesContainer(ambientServices);
        }

        /// <summary>
        /// Configures the worker before adding the background worker.
        ///  Here is the place where logging should be initialized.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="config">The services configuration callback.</param>
        /// <returns>
        /// The provided <see cref="WebAssemblyHostBuilder"/>.
        /// </returns>
        protected virtual WebAssemblyHostBuilder PreConfigureWorker(WebAssemblyHostBuilder hostBuilder, Action<IServiceCollection>? config)
        {
            hostBuilder.RootComponents.Add<TApp>("#app");

            hostBuilder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostBuilder.HostEnvironment.BaseAddress) });

            config?.Invoke(hostBuilder.Services);

            return hostBuilder;
        }

        /// <summary>
        /// Configures the worker. Here is the place where the Windows service or the Linux daemon
        /// should be initialized.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="config">The services configuration callback.</param>
        /// <returns>
        /// The provided <see cref="WebAssemblyHostBuilder"/>.
        /// </returns>
        protected virtual WebAssemblyHostBuilder ConfigureWorker(WebAssemblyHostBuilder hostBuilder, Action<IServiceCollection>? config)
        {
            config?.Invoke(hostBuilder.Services);

            return hostBuilder;
        }

        /// <summary>
        /// Configures the worker after configuring the ambient services.
        /// Here is the place to do whatever initialization is necessary before actually
        /// going into the bootstrapping procedure.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>
        /// The provided <see cref="WebAssemblyHostBuilder"/>.
        /// </returns>
        protected virtual WebAssemblyHostBuilder PostConfigureWorker(WebAssemblyHostBuilder hostBuilder)
        {
            return hostBuilder;
        }
    }
}
