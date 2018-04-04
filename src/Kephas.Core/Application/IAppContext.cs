// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// Interface for application context.
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
        /// Gets a function for signalling the application to shutdown.
        /// </summary>
        /// <value>
        /// The signal shutdown.
        /// </value>
        Func<IContext, Task<IAppContext>> SignalShutdown { get; }
    }
}