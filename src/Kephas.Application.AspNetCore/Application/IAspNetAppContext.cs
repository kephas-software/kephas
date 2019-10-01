// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAspNetAppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAspNetAppContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Application
{
    using Kephas.Application;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Contract for the OWIN application context.
    /// </summary>
    public interface IAspNetAppContext : IAppContext
    {
        /// <summary>
        /// Gets the application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        IApplicationBuilder AppBuilder { get; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the ASP.NET configuration.
        /// </summary>
        /// <value>
        /// The ASP.NET configuration.
        /// </value>
        IConfiguration Configuration { get; }
    }
}