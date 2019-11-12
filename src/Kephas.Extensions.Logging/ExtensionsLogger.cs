// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the extensions logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using System;
    using System.Text;

    using Kephas.Logging;

    /// <summary>
    /// The extensions logger.
    /// </summary>
    public class ExtensionsLogger : ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ExtensionsLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            this.logger = logger;
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
            return this.logger.IsEnabled(this.ToLogLevel(level));
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments for the message format.</param>
        public void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            this.logger.Log(this.ToLogLevel(level), default, messageFormat, exception, (msg, ex) => this.Format(msg, ex, args));
        }

        private string Format(string messageFormat, Exception exception, params object[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(messageFormat, args);
            if (exception != null)
            {
                sb.AppendLine().AppendLine(exception.Message).AppendLine(exception.StackTrace);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a logLevel to a log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <returns>
        /// LogLevel as a Microsoft.Extensions.Logging.LogLevel.
        /// </returns>
        private Microsoft.Extensions.Logging.LogLevel ToLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
                case LogLevel.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                case LogLevel.Warning:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                case LogLevel.Info:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                case LogLevel.Debug:
                    return Microsoft.Extensions.Logging.LogLevel.Debug;
                case LogLevel.Trace:
                    return Microsoft.Extensions.Logging.LogLevel.Trace;
                default:
                    return Microsoft.Extensions.Logging.LogLevel.Trace;
            }
        }
    }
}
