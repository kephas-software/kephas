// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Server
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.AspNetCore.Blazor.InteractiveTests.Server.Extensions;
    using Kephas.Cryptography;
    using Kephas.Extensions.Hosting;
    using Kephas.Services.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static async Task Main(string[] args) =>
            await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var appArgs = new AppArgs(args);
            var builder = Host.CreateDefaultBuilder(args);

            var servicesBuilder = new AppServiceCollectionBuilder();

            return builder.ConfigureAppServices(
                    servicesBuilder,
                    appArgs,
                    (context, services, ambient) => ambient.SetupAmbientServices(CreateEncryptionService, context.Configuration))
                .ConfigureWebHostDefaults(
                    webBuilder => webBuilder
                        .UseKestrel()
                        .UseStartup<Startup>());
        }

        private static IEncryptionService CreateEncryptionService(IAppServiceCollectionBuilder servicesBuilder)
        {
            return new EncryptionService(() => new EncryptionContext(servicesBuilder.Injector));
        }

        private class EncryptionService : AesEncryptionService
        {
            public EncryptionService(Func<IEncryptionContext> contextCtor) : base(contextCtor)
            {
            }
        }
    }
}