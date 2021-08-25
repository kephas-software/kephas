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

    using Kephas.Commands;
    using Kephas.Services;

    /// <summary>
    /// The default application context.
    /// </summary>
    public class AppContext : Context, IAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public AppContext(
            IAmbientServices ambientServices,
            IAppRuntime? appRuntime = null,
            IAppArgs? appArgs = null)
            : base(ambientServices)
        {
            this.AppRuntime = appRuntime ?? this.AmbientServices?.AppRuntime!;
            this.AppArgs = appArgs ?? new AppArgs();
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        public IAppRuntime AppRuntime { get; }

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
    }
}