// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET6_0_OR_GREATER

namespace Kephas.Application.AspNetCore
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Operations;
    using Kephas.Services.Builder;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// The startup class of a web application.
    /// </summary>
    public class WebApp : StartupAppBase
    {
        private readonly WebApplicationBuilder builder;
        private WebApplication? app;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApp"/> class.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="builderOptions">The web application builder options.</param>
        public WebApp(IAppArgs appArgs, Action<WebApplicationBuilder>? builderOptions = null)
            : this(CreateBuilder(builderOptions, appArgs), appArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApp"/> class.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        /// <param name="appArgs">The application arguments.</param>
        protected WebApp(WebApplicationBuilder builder, IAppArgs appArgs)
            : base(builder.Environment, builder.Configuration, appArgs: appArgs)
        {
            this.builder = builder;
        }

        /// <summary>
        /// The main loop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task yielding the <see cref="MainLoopResult"/>.</returns>
        protected override async Task<MainLoopResult> Main(CancellationToken cancellationToken)
        {
            await this.app!.RunAsync().PreserveThreadContext();
            return new MainLoopResult(true.ToOperationResult(), AppShutdownInstruction.Shutdown);
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
            this.app = this.builder.Build();

            this.Configure(this.app, this.app.Lifetime);

            return this.app.Services;
        }

        /// <summary>
        /// Runs the application asynchronously in service mode.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        protected override Task<AppRunResult> RunServiceAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AppRunResult(this.AppContext, AppShutdownInstruction.Shutdown));
        }

        private static WebApplicationBuilder CreateBuilder(Action<WebApplicationBuilder>? builderOptions, IAppArgs appArgs)
        {
            var builder = WebApplication.CreateBuilder(appArgs.ToCommandArgs().ToArray());
            builderOptions?.Invoke(builder);
            return builder;
        }
    }
}

#endif