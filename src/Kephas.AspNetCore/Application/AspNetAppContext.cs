// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetAppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    using AppContext = Kephas.Application.AppContext;

    /// <summary>
    /// The OWIN web application context.
    /// </summary>
    public class AspNetAppContext : AppContext, IAspNetAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetAppContext"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="configuration">The ASP.NET configuration.</param>
        /// <param name="ambientServices">Optional. The ambient services. If not provided,
        ///                               <see cref="AmbientServices.Instance"/> will be considered.</param>
        /// <param name="appManifest">Optional. The application manifest.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public AspNetAppContext(
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            IAmbientServices ambientServices = null,
            IAppManifest appManifest = null,
            IAppArgs appArgs = null)
            : base(ambientServices, appManifest, appArgs)
        {
            this.HostingEnvironment = hostingEnvironment;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        public IApplicationBuilder AppBuilder => this.AmbientServices.GetService<IApplicationBuilder>();

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the ASP.NET configuration.
        /// </summary>
        /// <value>
        /// The ASP.NET configuration.
        /// </value>
        public IConfiguration Configuration { get; }
    }
}