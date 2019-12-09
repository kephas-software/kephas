// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Log manager for NLog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using System;
    using System.Collections.Concurrent;

    using global::NLog;
    using global::NLog.Config;

    /// <summary>
    /// Log manager for NLog.
    /// </summary>
    public class NLogManager : ILogManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, Logging.ILogger> loggers = new ConcurrentDictionary<string, Logging.ILogger>();
        private LogFactory logFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogManager"/> class.
        /// </summary>
        /// <param name="configuration">Optional. The configuration.</param>
        public NLogManager(LoggingConfiguration configuration = null)
        {
            this.logFactory = new LogFactory(configuration ?? LogManager.Configuration);
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
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public Logging.ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, this.CreateLogger);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual Logging.ILogger CreateLogger(string loggerName)
        {
            var nlogger = this.logFactory.GetLogger(loggerName);
            return new NLogger(nlogger);
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
            this.logFactory.Dispose();
        }
    }
}