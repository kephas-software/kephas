﻿// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Commands;
    using Kephas.Services;

    /// <summary>
    /// Contract for application contextual information.
    /// </summary>
    public interface IAppContext : IContext
    {
        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the application arguments passed typically as command line arguments.
        /// </summary>
        /// <value>
        /// The application arguments.
        /// </value>
        IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets or sets the application root exception.
        /// </summary>
        /// <value>
        /// The application root exception.
        /// </value>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the application result.
        /// </summary>
        /// <remarks>
        /// For console applications this is typically the integer return code.
        /// </remarks>
        /// <value>
        /// The application result.
        /// </value>
        object AppResult { get; set; }
    }
}