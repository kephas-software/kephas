// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A log4net logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Log4Net
{
    using System;

    using Kephas.Logging.Log4Net.Internal;

    /// <summary>
    /// A log4net logger.
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        private readonly log4net.Core.ILogger logger;
        private readonly StructuredLogEntryProvider entryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        protected internal Log4NetLogger(log4net.Core.ILogger logger)
        {
            this.logger = logger;
            this.entryProvider = new StructuredLogEntryProvider();
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
            return this.logger.IsEnabledFor(this.ToLevel(level));
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">        The logging level.</param>
        /// <param name="exception">    The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">         A variable-length parameters list containing arguments.</param>
        public void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            var (message, positionalArgs, _) = this.entryProvider.GetLogEntry(messageFormat, args);
            message = positionalArgs == null || positionalArgs.Length == 0 ? message : string.Format(message, positionalArgs);

            this.logger.Log(this.GetType(), this.ToLevel(level), message, exception);
        }

        private log4net.Core.Level ToLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    return log4net.Core.Level.Critical;
                case LogLevel.Error:
                    return log4net.Core.Level.Error;
                case LogLevel.Warning:
                    return log4net.Core.Level.Warn;
                case LogLevel.Info:
                    return log4net.Core.Level.Info;
                case LogLevel.Debug:
                    return log4net.Core.Level.Debug;
                case LogLevel.Trace:
                    return log4net.Core.Level.Trace;
                default:
                    return log4net.Core.Level.Off;
            }
        }
    }
}