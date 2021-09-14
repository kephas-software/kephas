// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.AspNetCore.InteractiveTests.Extensions;
    using Kephas.Cryptography;
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
            var appArgs = new AppArgs(args);
            var ambientServices = new AmbientServices();
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(b => b.AddJsonFile("appSettings.json"))
                .ConfigureAmbientServices(
                    ambientServices,
                    args,
                    (services, ambient) => ambient.SetupAmbientServices(CreateEncryptionService, services.TryGetStartupService<IConfiguration>()))
                .ConfigureWebHostDefaults(
                    webBuilder => webBuilder
                        .UseUrls("http://*:5100", "https://*:5101")
                        .UseStartup<StartupApp>());
        }

        private static IEncryptionService CreateEncryptionService(IAmbientServices ambientServices)
        {
            return new EncryptionService(() => new EncryptionContext(ambientServices.Injector));
        }

        private class EncryptionService : AesEncryptionService
        {
            public EncryptionService(Func<IEncryptionContext> contextCtor) : base(contextCtor)
            {
            }
        }
    }
}
