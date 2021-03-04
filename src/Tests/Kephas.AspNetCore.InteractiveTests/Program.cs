// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests
{
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.InteractiveTests;
    using Kephas.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var ambientServices = new AmbientServices();
            return Host.CreateDefaultBuilder(args)
                    .ConfigureAmbientServices(ambientServices, args, PreconfigureAmbientServices)
                    .ConfigureWebHostDefaults(
                        webBuilder => webBuilder
                                        .UseAmbientServices(ambientServices)
                                        .UseUrls("http://*:5100", "https://*:5101")
                                        .UseStartup<StartupApp>())
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
