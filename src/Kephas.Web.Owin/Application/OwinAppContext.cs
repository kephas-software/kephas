// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Owin.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Application;

    using global::Owin;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    using AppContext = Kephas.Application.AppContext;

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
        /// <param name="appManifest">The application manifest (optional).</param>
        /// <param name="appArgs">The application arguments (optional).</param>
        /// <param name="signalShutdown">Function for signalling the application shutdown.</param>
        public OwinAppContext(
            IAppBuilder appBuilder,
            IAmbientServices ambientServices = null,
            IAppManifest appManifest = null,
            string[] appArgs = null,
            Func<IContext, Task<IAppContext>> signalShutdown = null)
            : base(ambientServices, appManifest, appArgs, signalShutdown)
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