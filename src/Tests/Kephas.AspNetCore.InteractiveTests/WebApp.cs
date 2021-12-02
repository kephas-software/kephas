// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET6_0

namespace Kephas.AspNetCore.InteractiveTests
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

    public class WebApp : StartupAppBase
    {
        private readonly WebApplicationBuilder builder;

        public WebApp(IAppArgs appArgs, Action<WebApplicationBuilder>? builderOptions = null)
            : this(CreateBuilder(builderOptions, appArgs), appArgs)
        {

        }

        protected WebApp(WebApplicationBuilder builder, IAppArgs appArgs)
            : base(builder.Environment, builder.Configuration, appArgs: appArgs)
        {
            this.builder = builder;
        }

        public override Task<(IAppContext? appContext, AppShutdownInstruction instruction)> RunAsync(Func<IAppArgs, Task<(IOperationResult result, AppShutdownInstruction instruction)>>? mainCallback = null, CancellationToken cancellationToken = default)
        {
            this.ConfigureServices(this.builder.Services);

            this.BeforeAppManagerInitialize(this.AppArgs);

            var app = this.builder.Build();

            this.Configure(app, app.Lifetime);

            async Task<(IOperationResult result, AppShutdownInstruction instruction)> DefaultMainCallback(IAppArgs appArgs)
            {
                var result = new OperationResult();
                await app!.RunAsync().PreserveThreadContext();
                return (result.Complete(), AppShutdownInstruction.Shutdown);
            }

            return base.RunAsync(mainCallback ?? DefaultMainCallback, cancellationToken);
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