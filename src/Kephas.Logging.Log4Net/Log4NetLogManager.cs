// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Log manager for log4net.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Log4Net
{
    using System.Collections.Concurrent;

    using log4net;

    /// <summary>
    /// Log manager for log4net.
    /// </summary>
    public class Log4NetLogManager : ILogManager
    {
        /// <summary>
        /// The default repository name.
        /// </summary>
        private const string DefaultRepositoryName = "Default";

        /// <summary>
        /// The loggers dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogManager"/> class.
        /// </summary>
        public Log4NetLogManager()
        {
            LogManager.CreateRepository(DefaultRepositoryName);
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
            var nlogger = LogManager.GetLogger(DefaultRepositoryName, loggerName);
            return new Log4NetLogger(nlogger);
        }
    }
}