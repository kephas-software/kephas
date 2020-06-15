// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetAppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using Kephas;
    using Kephas.Application;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Kephas.Commands;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The ASP.NET application context.
    /// </summary>
    public class AspNetAppContext : AppContext, IAspNetAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetAppContext"/> class.
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="configuration">The ASP.NET configuration.</param>
        /// <param name="ambientServices">Optional. The ambient services. If not provided then
        ///                               a new instance of <see cref="AmbientServices"/> will be created and used.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public AspNetAppContext(
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration,
            IAmbientServices ambientServices = null,
            IAppRuntime appRuntime = null,
            IArgs appArgs = null)
            : base(ambientServices, appRuntime, appArgs)
        {
            this.HostEnvironment = hostEnvironment;
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
        public IWebHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the ASP.NET configuration.
        /// </summary>
        /// <value>
        /// The ASP.NET configuration.
        /// </value>
        public IConfiguration Configuration { get; }
    }
}