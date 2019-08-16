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
    using System.Collections.Concurrent;

    using global::Serilog;
    using global::Serilog.Core;

    /// <summary>
    /// The Serilog log manager.
    /// </summary>
    public class SerilogManager : ILogManager
    {
        private readonly LoggerConfiguration configuration;

        private Logger rootLogger;

        /// <summary>
        /// The loggers dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, global::Kephas.Logging.ILogger> loggers = new ConcurrentDictionary<string, global::Kephas.Logging.ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogManager"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public SerilogManager(LoggerConfiguration configuration)
        {
            this.configuration = configuration ?? new LoggerConfiguration();
            this.rootLogger = this.configuration.CreateLogger();
        }

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
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual global::Kephas.Logging.ILogger CreateLogger(string loggerName)
        {
            var logger = new SerilogLogger(loggerName, this.rootLogger);
            return logger;
        }
    }
}