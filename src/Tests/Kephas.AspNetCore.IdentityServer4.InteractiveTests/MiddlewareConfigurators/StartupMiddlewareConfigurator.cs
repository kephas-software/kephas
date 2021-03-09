// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupMiddlewareConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.MiddlewareConfigurators
{
    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    [ProcessingPriority(Priority.Highest + 100)]
    public class StartupMiddlewareConfigurator : IMiddlewareConfigurator
    {
        public void Configure(IAspNetAppContext appContext)
        {
            var app = appContext.AppBuilder;
            var env = appContext.HostEnvironment;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
        }
    }
}
