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
        /// Gets or sets the minimum level.
        /// </summary>
        public LogLevel MinimumLevel { get; set; }

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
        /// Converts the log level to the log4net log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The log4net log level.</returns>
        protected internal static log4net.Core.Level ToLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Fatal => log4net.Core.Level.Critical,
                LogLevel.Error => log4net.Core.Level.Error,
                LogLevel.Warning => log4net.Core.Level.Warn,
                LogLevel.Info => log4net.Core.Level.Info,
                LogLevel.Debug => log4net.Core.Level.Debug,
                LogLevel.Trace => log4net.Core.Level.Trace,
                _ => log4net.Core.Level.Off
            };
        }

        /// <summary>
        /// Converts the log4net log level to log level.
        /// </summary>
        /// <param name="level">The log4net log level.</param>
        /// <returns>The log level.</returns>
        protected internal static LogLevel ToLogLevel(log4net.Core.Level level)
        {
            if (level == log4net.Core.Level.Critical)
            {
                return Logging.LogLevel.Fatal;
            }

            if (level == log4net.Core.Level.Error)
            {
                return Logging.LogLevel.Error;
            }

            if (level == log4net.Core.Level.Warn)
            {
                return Logging.LogLevel.Warning;
            }

            if (level == log4net.Core.Level.Info)
            {
                return Logging.LogLevel.Info;
            }

            if (level == log4net.Core.Level.Debug)
            {
                return Logging.LogLevel.Debug;
            }

            if (level == log4net.Core.Level.Trace)
            {
                return Logging.LogLevel.Trace;
            }

            return Logging.LogLevel.Trace;
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