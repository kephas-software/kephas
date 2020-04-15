// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serilog manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using System;
    using System.Collections.Concurrent;

    using global::Serilog;
    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    /// The Serilog log manager.
    /// </summary>
    public class SerilogManager : ILogManager, IDisposable
    {
        private readonly LoggerConfiguration configuration;
        private readonly LoggingLevelSwitch levelSwitch;
        private readonly ConcurrentDictionary<string, global::Kephas.Logging.ILogger> loggers = new ConcurrentDictionary<string, global::Kephas.Logging.ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogManager"/> class.
        /// </summary>
        /// <param name="configuration">Optional. The configuration.</param>
        /// <param name="levelSwitch">Optional. The logging level switch.</param>
        public SerilogManager(LoggerConfiguration? configuration = null, LoggingLevelSwitch? levelSwitch = null)
        {
            this.configuration = configuration ?? new LoggerConfiguration();
            this.levelSwitch = levelSwitch ?? new LoggingLevelSwitch();
            this.configuration.MinimumLevel.ControlledBy(this.levelSwitch);
            this.RootLogger = this.configuration.CreateLogger();
        }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public LogLevel MinimumLevel
        {
            get => ToLogLevel(this.levelSwitch.MinimumLevel);
            set => this.levelSwitch.MinimumLevel = ToLogEventLevel(value);
        }

        /// <summary>
        /// Gets the root logger.
        /// </summary>
        /// <value>
        /// The root logger.
        /// </value>
        protected Logger RootLogger { get; }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        public global::Kephas.Logging.ILogger GetLogger(string loggerName)
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
        /// Converts the log level to Serilog log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The Serilog log level.</returns>
        protected internal static LogEventLevel ToLogEventLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Fatal => LogEventLevel.Fatal,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Info => LogEventLevel.Information,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Trace => LogEventLevel.Verbose,
                _ => LogEventLevel.Verbose
            };
        }

        /// <summary>
        /// Converts the Serilog log level to log level.
        /// </summary>
        /// <param name="level">The Serilog log level.</param>
        /// <returns>The log level.</returns>
        protected internal static LogLevel ToLogLevel(LogEventLevel level)
        {
            return level switch
            {
                LogEventLevel.Fatal => LogLevel.Fatal,
                LogEventLevel.Error => LogLevel.Error,
                LogEventLevel.Warning => LogLevel.Warning,
                LogEventLevel.Information => LogLevel.Info,
                LogEventLevel.Debug => LogLevel.Debug,
                LogEventLevel.Verbose => LogLevel.Trace,
                _ => LogLevel.Trace
            };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.loggers.Clear();
            this.RootLogger.Dispose();
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual global::Kephas.Logging.ILogger CreateLogger(string loggerName)
        {
            var logger = new SerilogLogger(loggerName, this.RootLogger);
            return logger;
        }
    }
}