// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the web host feature manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using global::ServiceStack.Logging;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.ServiceStack.Hosting;
    using Kephas.ServiceStack.Logging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Manager of the web host features.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    [FeatureInfo(ServiceStackFeature.WebHost)]
    public class WebHostFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// The web host factory.
        /// </summary>
        private readonly IExportFactory<IWebHost> webHostFactory;

        /// <summary>
        /// The web host.
        /// </summary>
        private IWebHost webHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostFeatureManager"/> class.
        /// </summary>
        /// <param name="webHostFactory">The web host factory.</param>
        public WebHostFeatureManager(IExportFactory<IWebHost> webHostFactory)
        {
            this.webHostFactory = webHostFactory;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<WebHostFeatureManager> Logger { get; set; }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            LogManager.LogFactory = new LogFactory(appContext.CompositionContext.GetExport<ILogManager>());

            // initialize the web host
            this.webHost = this.webHostFactory.CreateExportedValue();
            await this.webHost.InitializeAsync(appContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Finalizes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            if (this.webHost != null)
            {
                await this.webHost.FinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
            }
        }
    }
}