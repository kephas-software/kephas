// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Application.AspNetCore;
    using Kephas.AspNetCore.IdentityServer4.InteractiveTests.Extensions;
    using Kephas.Cryptography;
    using Kephas.Extensions.Hosting;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var appLifetimeTokenSource = new CancellationTokenSource();
                var appArgs = new AppArgs(args);
                await CreateHostBuilder(args, appArgs, appLifetimeTokenSource).Build().RunAsync(appLifetimeTokenSource.Token).PreserveThreadContext();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 8;
            }
        }

        /// <summary>
        /// Creates host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="appLifetimeTokenSource">The app lifetime token source.</param>
        /// <returns>
        /// The new host builder.
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args, IAppArgs appArgs, CancellationTokenSource appLifetimeTokenSource)
        {
            var ambientServices = new AmbientServices();
            var hostBuilder = Host.CreateDefaultBuilder(args)
                                .ConfigureAppConfiguration((hostingContext, config) =>
                                    {
                                        var env = hostingContext.HostingEnvironment;
                                    })
                                .ConfigureAmbientServices(
                                    ambientServices,
                                    appArgs,
                                    ambient => ambient.BuildWithAutofac(),
                                    (services, ambient) => ambient.SetupAmbientServices(appArgs, CreateEncryptionService, services.TryGetStartupService<IConfiguration>(), appLifetimeTokenSource))
                                .ConfigureWebHostDefaults(
                                    webBuilder => webBuilder
                                                    .UseStartup<Startup>()
                                                    .UseKestrel(opts =>
                                                    {
                                                        webBuilder.UseAppUrls(ambientServices, opts);

                                                        if (appArgs.RunAsService)
                                                        {
                                                            opts.UseSystemd();
                                                        }
                                                    }));

            if (appArgs.RunAsService)
            {
                hostBuilder.UseWindowsService();
                hostBuilder.UseSystemd();
            }

            return hostBuilder;
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
