// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A web application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Application
{
    using System.Diagnostics.Contracts;

    using Kephas.Application;

    using Microsoft.AspNet.Builder;

    /// <summary>
    /// A web application context.
    /// </summary>
    public class WebAppContext : AppContext, IWebAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppContext"/> class.
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder.
        /// </param>
        public WebAppContext(IApplicationBuilder appBuilder)
        {
            Contract.Requires(appBuilder != null);

            this.AppBuilder = appBuilder;
        }

        /// <summary>
        /// Gets the application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        public IApplicationBuilder AppBuilder { get; }
    }
}