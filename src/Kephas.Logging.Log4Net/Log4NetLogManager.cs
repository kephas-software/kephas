// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Log manager for log4net.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Log4Net
{
    using System;
    using System.Collections.Concurrent;

    using log4net;
    using log4net.Repository;

    /// <summary>
    /// Log manager for log4net.
    /// </summary>
    public class Log4NetLogManager : ILogManager, IDisposable
    {
        /// <summary>
        /// The default repository name.
        /// </summary>
        private const string DefaultRepositoryName = "Default";

        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();
        private ILoggerRepository rootRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogManager"/> class.
        /// </summary>
        public Log4NetLogManager()
        {
            this.rootRepository = LogManager.CreateRepository(DefaultRepositoryName);
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
        public ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, this.CreateLogger);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual ILogger CreateLogger(string loggerName)
        {
            var nlogger = this.rootRepository.GetLogger(loggerName);
            return new Log4NetLogger(nlogger);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Logging.Log4Net.Log4NetLogManager and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.loggers.Clear();
            this.rootRepository.Shutdown();
        }
    }
}