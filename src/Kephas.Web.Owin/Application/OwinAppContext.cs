// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the owin application context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Owin.Application
{
    using System.Diagnostics.Contracts;

    using global::Owin;

    using Kephas.Application;

    /// <summary>
    /// The OWIN web application context.
    /// </summary>
    public class OwinAppContext : AppContext, IOwinAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwinAppContext"/> class.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        public OwinAppContext(IAppBuilder appBuilder)
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
        public IAppBuilder AppBuilder { get; }
    }
}