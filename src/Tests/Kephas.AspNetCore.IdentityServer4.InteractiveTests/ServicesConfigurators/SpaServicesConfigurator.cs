// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.ServicesConfigurators
{
    using System.Linq;

    using Kephas;
    using Kephas.AspNetCore.IdentityServer4.InteractiveTests.Extensions;
    using Kephas.Collections;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.Extensions.DependencyInjection;

    public class SpaServicesConfigurator : IServicesConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
        {
            var appAssemblies = ambientServices.GetAppAssemblies();
            var assemblyParts = appAssemblies.Select(asm => (ApplicationPart)new AssemblyPart(asm));

            services.AddControllersWithViews()
                .ConfigureApplicationPartManager(m => m.ApplicationParts.AddRange(assemblyParts))
                .AddNewtonsoftJson(opts => ambientServices.ConfigureJsonSerialization(opts.SerializerSettings));
            services.AddRazorPages();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }
    }
}
