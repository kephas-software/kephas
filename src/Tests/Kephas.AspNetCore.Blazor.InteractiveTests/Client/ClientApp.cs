// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Logging;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The client application.
    /// </summary>
    /// <typeparam name="TApp">The type of the Razor entry point component (typically App).</typeparam>
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
            : base(new AmbientServices(), appArgs: appArgs, appLifetimeTokenSource: null, containerBuilder)
        {
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
        /// Runs the application asynchronously.
        /// </summary>
        /// <param name="mainCallback">
        /// Optional. The callback for the main function.
        /// If not provided, the service implementing <see cref="IAppMainLoop"/> will be invoked,
        /// otherwise the application will end.
        /// </param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override Task<(IAppContext? appContext, AppShutdownInstruction instruction)> RunAsync(
            Func<IAppArgs, Task<(IOperationResult result, AppShutdownInstruction instruction)>>? mainCallback = null,
            CancellationToken cancellationToken = default)
        {
            this.HostBuilder = this.CreateHostBuilder(this.AppArgs);

            this.HostBuilder.ConfigureContainer(new InjectionServiceProviderFactory(this.AmbientServices));

            this.ConfigureHost(this.HostBuilder);

            this.AmbientServices
                .WithServiceCollection(this.HostBuilder.Services)
                .ConfigureExtensionsLogging();

            return base.RunAsync(mainCallback ?? this.RunAsync, cancellationToken);
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
        /// Runs the host asynchronously.
        /// </summary>
        /// <param name="appArgs">The application argument.</param>
        /// <returns>A tuple providing the result and the shutdown instruction.</returns>
        protected virtual async Task<(IOperationResult result, AppShutdownInstruction instruction)> RunAsync(IAppArgs appArgs)
        {
            this.Host = this.HostBuilder!.Build();
            await this.Host.RunAsync().PreserveThreadContext();
            return (0.ToOperationResult(), AppShutdownInstruction.Shutdown);
        }

        /// <summary>
        /// Configures the host. This is the place where the root component and all the services
        /// should be added and configured.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <returns>
        /// The provided <see cref="WebAssemblyHostBuilder"/>.
        /// </returns>
        protected virtual WebAssemblyHostBuilder ConfigureHost(WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<TApp>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<IAppManager, ClientAppManager>();

            return builder;
        }
    }
}
