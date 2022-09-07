// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebUIServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.ServicesConfigurators
{
    using System.Linq;

    using Kephas;
    using Kephas.Application.AspNetCore;
    using Kephas.AspNetCore.IdentityServer4.InteractiveTests.Extensions;
    using Kephas.Collections;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.Extensions.DependencyInjection;

    public class WebUIServicesConfigurator : IServicesConfigurator
    {
        public void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
        {
            var appRuntime = ambientServices.GetAppRuntime();
            services.AddControllersWithViews()
                .ConfigureApplicationPartManager(appRuntime)
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
