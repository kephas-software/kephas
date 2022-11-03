// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using Kephas.Application;
    using Kephas.Services.Builder;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The ASP.NET application context.
    /// </summary>
    public class WebAppContext : AppContext, IWebAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppContext"/> class.
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="configuration">The ASP.NET configuration.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public WebAppContext(
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration,
            IAppServiceCollectionBuilder servicesBuilder,
            IAppArgs? appArgs = null)
            : base(servicesBuilder, appArgs)
        {
            this.HostEnvironment = hostEnvironment;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the application builder.
        /// </summary>
        public IApplicationBuilder App { get; set; }

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