// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebAppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWebAppContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using Kephas.Application;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Contract for the OWIN application context.
    /// </summary>
    public interface IWebAppContext : IAppContext
    {
        /// <summary>
        /// Gets or sets the application builder.
        /// </summary>
        IApplicationBuilder App { get; set; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        IWebHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the ASP.NET configuration.
        /// </summary>
        /// <value>
        /// The ASP.NET configuration.
        /// </value>
        IConfiguration Configuration { get; }
    }
}