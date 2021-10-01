// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Manager service for loggers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Manager service for loggers.
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Gets or sets the global minimum level.
        /// </summary>
        LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        ILogger GetLogger(string loggerName);
    }

    /// <summary>
    /// Extension methods for <see cref="ILogManager"/>.
    /// </summary>
    public static class LogManagerExtensions
    {
        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="logManager">The logger factory.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger(this ILogManager logManager, Type type)
        {
            Requires.NotNull(logManager, nameof(logManager));
            type = type ?? throw new ArgumentNullException(nameof(type));

            return logManager.GetLogger(type.FullName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="TTarget">The type for which the logger should be created.</typeparam>
        /// <param name="logManager">The logger factory.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger<TTarget>(this ILogManager logManager)
        {
            Requires.NotNull(logManager, nameof(logManager));

            return logManager.GetLogger(typeof(TTarget));
        }
    }
}