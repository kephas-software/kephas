// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndpointsMiddlewareConfigurator.cs" company="Kephas Software SRL">
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

    [ProcessingPriority(Priority.BelowNormal)]
    public class EndpointsMiddlewareConfigurator : IMiddlewareConfigurator
    {
        public void Configure(IAspNetAppContext appContext)
        {
            var app = appContext.AppBuilder;
            var env = appContext.HostEnvironment;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
