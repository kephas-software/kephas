// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="configuration">The ASP.NET configuration.</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided,
        ///                               <see cref="AmbientServices.Instance"/> will be considered.</param>
        /// <param name="appManifest">The application manifest (optional).</param>
        /// <param name="appArgs">The application arguments (optional).</param>
        /// <param name="signalShutdown">Function for signalling the application shutdown (optional).</param>
        public AspNetAppContext(
            IApplicationBuilder appBuilder,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            IAmbientServices ambientServices = null,
            IAppManifest appManifest = null,
            string[] appArgs = null,
            Func<IContext, Task<IAppContext>> signalShutdown = null)
            : base(ambientServices, appManifest, appArgs, signalShutdown)
        {
            Requires.NotNull(appBuilder, nameof(appBuilder));

            this.AppBuilder = appBuilder;
            this.HostingEnvironment = hostingEnvironment;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        public IApplicationBuilder AppBuilder { get; }

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