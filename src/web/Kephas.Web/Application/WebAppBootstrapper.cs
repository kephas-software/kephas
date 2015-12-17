// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebAppBootstrapper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A web application bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;
    using Kephas.Web.Resources;

    /// <summary>
    /// A web application bootstrapper.
    /// </summary>
    [OverridePriority(Priority.Normal)]
    public class WebAppBootstrapper : DefaultAppBootstrapper
    {
        /// <summary>
        /// Monitors the initialization state.
        /// </summary>
        private readonly InitializationMonitor<IAppBootstrapper, WebAppBootstrapper> initialization = new InitializationMonitor<IAppBootstrapper, WebAppBootstrapper>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppBootstrapper"/> class.
        /// </summary>
        /// <param name="appIntializerFactories">The app intializer factories.</param>
        public WebAppBootstrapper(ICollection<IExportFactory<IAppInitializer, AppServiceMetadata>> appIntializerFactories)
            : base(appIntializerFactories)
        {
        }

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task StartAsync(IAppContext appContext, CancellationToken cancellationToken = new CancellationToken())
        {
            var webAppContext = appContext as IWebAppContext;
            if (webAppContext == null)
            {
                throw new ArgumentException($"The application context does not implement {nameof(IWebAppContext)}.");
            }

            if (webAppContext.AppBuilder == null)
            {
                throw new ArgumentException($"The web application context does not have the {nameof(IWebAppContext.AppBuilder)} property set.");
            }

            // starts the bootstrapper asynchronously and gets the running task
            // to await for it in the pipeline.
            this.initialization.Start();
            var startupTask = base.StartAsync(appContext, cancellationToken);

            webAppContext.AppBuilder.Use(
                next => async context =>
                    {
                        if (!this.initialization.IsCompleted)
                        {
                            try
                            {
                                // wait for initialization completion.
                                await startupTask.WithServerThreadingContext();
                            }
                            catch (Exception ex)
                            {
                                this.initialization.Fault(ex);
                                this.Logger.Fatal(ex, Strings.WebAppBootstrapper_Initialization_Exception);
                                // TODO return a failure response to the client
                            }
                        }

                        await next(context).WithServerThreadingContext();
                    });

            return startupTask;
        }
    }
}