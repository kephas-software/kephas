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

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogManager"/> class.
        /// </summary>
        /// <param name="configuration">Optional. The configuration.</param>
        public NLogManager(LoggingConfiguration? configuration = null)
            : this(configuration != null ? new LogFactory(configuration) : LogManager.LogFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogManager"/> class.
        /// </summary>
        /// <param name="logFactory">Active NLog LogFactory.</param>
        public NLogManager(LogFactory logFactory)
        {
            this.LogFactory = logFactory;
        }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public global::Kephas.Logging.LogLevel MinimumLevel
        {
            get => ToLogLevel(this.LogFactory.GlobalThreshold);
            set => this.LogFactory.GlobalThreshold = ToNLogLevel(value);
        }

        /// <summary>
        /// Gets the log factory.
        /// </summary>
        /// <value>
        /// The log factory.
        /// </value>
        protected LogFactory LogFactory { get; }

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
        /// Converts the log level to NLog log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The NLog log level.</returns>
        protected internal static Logging.LogLevel ToLogLevel(LogLevel level)
        {
            if (level == LogLevel.Fatal)
            {
                return Logging.LogLevel.Fatal;
            }

            if (level == LogLevel.Error)
            {
                return Logging.LogLevel.Error;
            }

            if (level == LogLevel.Warn)
            {
                return Logging.LogLevel.Warning;
            }

            if (level == LogLevel.Info)
            {
                return Logging.LogLevel.Info;
            }

            if (level == LogLevel.Debug)
            {
                return Logging.LogLevel.Debug;
            }

            if (level == LogLevel.Trace)
            {
                return Logging.LogLevel.Trace;
            }

            return Logging.LogLevel.Trace;
        }

        /// <summary>
        /// Converts the log level to NLog log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The NLog log level.</returns>
        protected internal static LogLevel ToNLogLevel(Logging.LogLevel level)
        {
            return level switch
            {
                Logging.LogLevel.Fatal => LogLevel.Fatal,
                Logging.LogLevel.Error => LogLevel.Error,
                Logging.LogLevel.Warning => LogLevel.Warn,
                Logging.LogLevel.Info => LogLevel.Info,
                Logging.LogLevel.Debug => LogLevel.Debug,
                Logging.LogLevel.Trace => LogLevel.Trace,
                _ => LogLevel.Off
            };
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual Logging.ILogger CreateLogger(string loggerName)
        {
            var nlogger = this.LogFactory.GetLogger(loggerName);
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
            this.LogFactory.Shutdown();
        }
    }
}