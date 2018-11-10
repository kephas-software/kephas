// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlLoggerAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the Npgsql logger adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Npgsql.Logging
{
    using System;

    using Kephas.Logging;

    using global::Npgsql.Logging;

    /// <summary>
    /// A logger adapter for NpgSql.
    /// </summary>
    public class NpgsqlLoggerAdapter : NpgsqlLogger
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlLoggerAdapter"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NpgsqlLoggerAdapter(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets a value indicating whether the provided logging level is enabled.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <returns>
        /// <c>true</c> if the logging level is enabled, <c>false</c> if not.
        /// </returns>
        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return this.logger.IsEnabled(this.GetLogLevel(level));
        }

        /// <summary>
        /// Logs the data at the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="connectorId">Identifier for the connector.</param>
        /// <param name="msg">The message.</param>
        /// <param name="exception">The exception to log (optional).</param>
        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            this.logger.Log(this.GetLogLevel(level), exception, msg);
        }

        /// <summary>
        /// Gets the Kephas log level.
        /// </summary>
        /// <param name="logLevel">The native log level.</param>
        /// <returns>
        /// The Kephas log level.
        /// </returns>
        private LogLevel GetLogLevel(NpgsqlLogLevel logLevel)
        {
            switch (logLevel)
            {
                case NpgsqlLogLevel.Fatal:
                    return LogLevel.Fatal;
                case NpgsqlLogLevel.Error:
                    return LogLevel.Error;
                case NpgsqlLogLevel.Warn:
                    return LogLevel.Warning;
                case NpgsqlLogLevel.Info:
                    return LogLevel.Info;
                case NpgsqlLogLevel.Debug:
                    return LogLevel.Debug;
                case NpgsqlLogLevel.Trace:
                    return LogLevel.Trace;
                default:
                    return LogLevel.Trace;
            }
        }
    }
}