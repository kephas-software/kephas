// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.AspNetCore.Blazor.InteractiveTests.Client.Application;
    using Serilog;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var clientApp = new ClientApp<App>(
                new AppArgs(args),
                a => a
                    .WithSerilogManager(GetLoggerConfiguration())
                    .WithStaticAppRuntime()
                    .BuildWithAutofac());

            await clientApp.RunAsync();
        }

        private static LoggerConfiguration GetLoggerConfiguration()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.BrowserConsole();
        }
    }
}
