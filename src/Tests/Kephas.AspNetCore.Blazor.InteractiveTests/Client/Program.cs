// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Logging;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var clientApp = new ClientApp<App>(
                ConfigureAmbientServices,
                new AppArgs(args));

            return await clientApp.RunAsync(1);
        }

        private static LoggerConfiguration GetLoggerConfiguration()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.BrowserConsole();
        }

        private static void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithSerilogManager(GetLoggerConfiguration())
                .WithStaticAppRuntime()
                .BuildWithLite();
        }
    }
}
