// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Log manager for NLog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using System.Collections.Concurrent;

    using global::NLog;

    /// <summary>
    /// Log manager for NLog.
    /// </summary>
    public class NLogManager : ILogManager
    {
        /// <summary>
        /// The loggers dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

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
            return new NLogger(nlogger);
        }
    }
}