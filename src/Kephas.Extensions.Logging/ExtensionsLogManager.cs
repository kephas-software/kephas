// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsLogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the extensions log manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using System;
    using System.Collections.Concurrent;

    using Kephas.Logging;
    using Microsoft.Extensions.Logging;

    using ILogger = Kephas.Logging.ILogger;

    /// <summary>
    /// Manager for extensions logs.
    /// </summary>
    public class ExtensionsLogManager : ILogManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ();
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsLogManager"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ExtensionsLogManager(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public Kephas.Logging.LogLevel MinimumLevel { get; set; } = Kephas.Logging.LogLevel.Trace;

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, this.CreateLogger);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual ILogger CreateLogger(string loggerName)
        {
            return new ExtensionsLogger(this.loggerFactory.CreateLogger(loggerName));
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Logging.NLog.NLogManager and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.loggers.Clear();
            this.loggerFactory.Dispose();
        }
    }
}
