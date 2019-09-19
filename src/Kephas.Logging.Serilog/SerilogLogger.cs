// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serilog logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using System;

    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    /// The Serilog logger adapter.
    /// </summary>
    public class SerilogLogger : ILogger
    {
        private readonly global::Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="rootLogger">The root logger.</param>
        internal SerilogLogger(string loggerName, Logger rootLogger)
        {
            this.logger = rootLogger.ForContext(Constants.SourceContextPropertyName, loggerName, false);
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <remarks>
        /// Note for implementors: the <paramref name="exception"/> may be <c>null</c>, so be cautious and handle this case too.
        /// For example, the <see cref="LoggerExtensions.Log(ILogger, LogLevel, string, object[])"/> extension method passes a <c>null</c> exception.
        /// </remarks>
        /// <param name="level">The logging level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments for the message format.</param>
        public void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            if (exception == null)
            {
                this.logger.Write(this.ToLogEventLevel(level), messageFormat, args);
            }
            else
            {
                this.logger.Write(this.ToLogEventLevel(level), exception, messageFormat, args);
            }
        }

        /// <summary>
        /// Indicates whether logging at the indicated level is enabled.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public bool IsEnabled(LogLevel level)
        {
            return this.logger.IsEnabled(this.ToLogEventLevel(level));
        }

        private LogEventLevel ToLogEventLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Info:
                    return LogEventLevel.Information;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}