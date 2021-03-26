// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaMiddlewareConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests.MiddlewareConfigurators
{
    using System;
    using System.IO;

    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Hosting;

    [ProcessingPriority(Priority.Low)]
    public class SpaMiddlewareConfigurator : IMiddlewareConfigurator
    {
        public void Configure(IAspNetAppContext appContext)
        {
            var app = appContext.AppBuilder;
            var env = appContext.HostEnvironment;
            var ambientServices = appContext.AmbientServices;

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                // see https://github.com/dotnet/aspnetcore/issues/6342#issue-395713046
                // and https://stackoverflow.com/questions/55845971/asp-net-core-and-react-gives-failed-to-start-npm-the-directory-name-is-inval
                spa.Options.SourcePath = Path.Join(env.ContentRootPath, this.GetClientAppPath(ambientServices));

                if (env.IsDevelopment())
                {
                    spa.Options.StartupTimeout = new TimeSpan(0, 2, 0);
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        /// <summary>
        /// Gets the client application path.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The client application path.</returns>
        protected virtual string GetClientAppPath(IAmbientServices ambientServices)
        {
            return "ClientApp";
        }
    }
}
