// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Kephas;
using Kephas.Application;
using Kephas.AspNetCore.InteractiveTests;
using Kephas.AspNetCore.InteractiveTests.Extensions;
using Kephas.Cryptography;
using Kephas.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#if NET6_0_OR_GREATER

var appArgs = new AppArgs(args);
var ambientServices = new AmbientServices();
await new WebApp(
        new AppArgs(args),
        builder =>
        {
            builder.Host
                .ConfigureAmbientServices(
                    ambientServices,
                    args,
                    ambient => ambient.BuildWithAutofac(),
                    (services, ambient) =>
                        ambient.SetupAmbientServices(
                            CreateEncryptionService,
                            services.TryGetStartupService<IConfiguration>()));
        })
    .RunAsync();

#else

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args)
{
    var appArgs = new AppArgs(args);
    var ambientServices = new AmbientServices();
    return Host.CreateDefaultBuilder(args)
        .ConfigureAmbientServices(
            ambientServices,
            args,
            ambient => ambient.BuildWithAutofac(),
            (services, ambient) =>
                ambient.SetupAmbientServices(CreateEncryptionService, services.TryGetStartupService<IConfiguration>()))
        .ConfigureWebHostDefaults(
            webBuilder => webBuilder
                .UseStartup<Startup>()
                .UseKestrel());
}

#endif

static IEncryptionService CreateEncryptionService(IAmbientServices ambientServices)
{
    return new EncryptionService(() => new EncryptionContext(ambientServices.Injector));
}

class EncryptionService : AesEncryptionService
{
    public EncryptionService(Func<IEncryptionContext> contextCtor) : base(contextCtor)
    {
    }
}