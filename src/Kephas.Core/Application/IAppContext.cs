// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Interface for application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Contract for application contextual information.
    /// </summary>
    public interface IAppContext : IContext
    {
        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        IAppManifest AppManifest { get; }

        /// <summary>
        /// Gets the application arguments passed typically as command line arguments.
        /// </summary>
        /// <value>
        /// The application arguments.
        /// </value>
        string[] AppArgs { get; }

        /// <summary>
        /// Gets or sets the application root exception.
        /// </summary>
        /// <value>
        /// The application root exception.
        /// </value>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets a function for signalling the application to shutdown.
        /// </summary>
        /// <value>
        /// The signal shutdown.
        /// </value>
        Func<IContext, Task<IAppContext>> SignalShutdown { get; }
    }
}