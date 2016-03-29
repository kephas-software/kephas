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
        /// The loggers dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, Logging.ILogger> loggers = new ConcurrentDictionary<string, Logging.ILogger>();

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
        private ILogger CreateLogger(string loggerName)
        {
            var nlogger = LogManager.GetLogger(loggerName);
            return new Log4NetLogger(nlogger);
        }
    }
}