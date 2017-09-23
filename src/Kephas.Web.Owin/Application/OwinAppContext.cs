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
    using Kephas.Application;

    using global::Owin;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// The OWIN web application context.
    /// </summary>
    public class OwinAppContext : AppContext, IOwinAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwinAppContext"/> class.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided, <see cref="AmbientServices.Instance"/> will be considered.</param>
        public OwinAppContext(IAppBuilder appBuilder, IAmbientServices ambientServices = null)
            : base(ambientServices)
        {
            Requires.NotNull(appBuilder, nameof(appBuilder));

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