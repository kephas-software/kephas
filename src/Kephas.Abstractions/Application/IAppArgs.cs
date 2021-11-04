// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Commands;
    using Kephas.Logging;

    /// <summary>
    /// Provides the commonly used arguments in applications.
    /// </summary>
    public interface IAppArgs : IArgs
    {
        /// <summary>
        /// Gets the application ID, if set.
        /// </summary>
        string? AppId { get; }

        /// <summary>
        /// Gets the application instance ID, if set.
        /// </summary>
        string? AppInstanceId { get; }

        /// <summary>
        /// Gets the ID of the root application instance, if set.
        /// </summary>
        string? RootAppInstanceId { get; }

        /// <summary>
        /// Gets the log level, if set.
        /// </summary>
        LogLevel? LogLevel { get; }

        /// <summary>
        /// Gets the environment name.
        /// </summary>
        public string? Environment { get; }

        /// <summary>
        /// Gets a value indicating whether the application is executed as a Windows service (or Linux daemon).
        /// </summary>
        bool RunAsService { get; }

        /// <summary>
        /// Gets a value indicating whether this application instance is the root.
        /// </summary>
        bool RunAsRoot { get; }

        /// <summary>
        /// Gets a value indicating whether the application is in development mode.
        /// </summary>
        bool IsDevelopment { get; }
    }
}