// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// The default application context.
    /// </summary>
    public class AppContext : Expando, IAppContext
    {
        private ILogger? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppContext"/> class.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public AppContext(
            IAppServiceCollection appServices,
            IAppArgs? appArgs = null)
        {
            this.AppServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            this.AppArgs = appArgs ?? new AppArgs();
        }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        public IAppServiceCollection AppServices { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        public IAppRuntime AppRuntime => this.AppServices.GetAppRuntime();

        /// <summary>
        /// Gets the application arguments passed typically as command line arguments.
        /// </summary>
        /// <value>
        /// The application arguments.
        /// </value>
        public IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets or sets the application root exception.
        /// </summary>
        /// <value>
        /// The application root exception.
        /// </value>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the application result.
        /// </summary>
        /// <value>
        /// The application result.
        /// </value>
        public object? AppResult { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger? Logger
        {
            get => this.logger ?? this.AppServices.GetServiceInstance<ILogManager>().GetLogger();
            init => this.logger = value;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        void IDisposable.Dispose()
        {
        }
    }
}