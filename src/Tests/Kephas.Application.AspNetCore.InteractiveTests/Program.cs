// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Serilog;

namespace Kephas.Application.AspNetCore.InteractiveTests
{
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Hosting;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilderFactory = new HostBuilderFactory(args);
            return hostBuilderFactory
                    .CreateHostBuilder(PreconfigureAmbientServices)
                    .ConfigureWebHostDefaults(
                        webBuilder => webBuilder
                                        .UseUrls("http://*:5100", "https://*:5101")
                                        .UseStartup<Startup>())
                    .ConfigureAppConfiguration(b => b.AddJsonFile("appSettings.json"));
        }

        private static void PreconfigureAmbientServices(HostBuilderContext context, IConfigurationBuilder configurationBuilder, IAmbientServices ambientServices)
        {
            var serilogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(context.Configuration);

            ambientServices
                .WithSerilogManager(serilogConfig)
                .WithDynamicAppRuntime();
        }
    }
}
