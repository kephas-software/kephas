﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Interface for application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Dynamic;
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
    }
}