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
    using Kephas.Operations;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// The startup class of a web application.
    /// </summary>
    public class WebApp : StartupAppBase
    {
        private readonly WebApplicationBuilder builder;

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
        /// Runs the application asynchronously.
        /// </summary>
        /// <param name="mainCallback">Not used. Preserved only to hide the base method.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        public override async Task<AppRunResult> RunAsync(
            Func<IAppArgs, Task<(IOperationResult result, AppShutdownInstruction instruction)>>? mainCallback = null,
            CancellationToken cancellationToken = default)
        {
            this.ConfigureServices(this.builder.Services);

            this.BeforeAppManagerInitialize(this.AppArgs);

            var app = this.builder.Build();

            this.Configure(app, app.Lifetime);

            await app.RunAsync().PreserveThreadContext();
            return new AppRunResult(this.AppContext, AppShutdownInstruction.Shutdown);
        }

        /// <summary>
        /// Runs the application asynchronously in service mode.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        protected override Task<AppRunResult> RunServiceAsync(CancellationToken cancellationToken = default)
            => base.RunAsync(null, cancellationToken);

        private static WebApplicationBuilder CreateBuilder(Action<WebApplicationBuilder>? builderOptions, IAppArgs appArgs)
        {
            var builder = WebApplication.CreateBuilder(appArgs.ToCommandArgs().ToArray());
            builderOptions?.Invoke(builder);
            return builder;
        }
    }
}

#endif