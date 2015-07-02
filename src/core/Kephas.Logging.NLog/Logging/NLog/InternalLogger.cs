// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalLogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   NLog logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using System;

    using global::NLog;

    /// <summary>
    /// NLog logger.
    /// </summary>
    internal class InternalLogger : Logging.ILogger
    {
        /// <summary>
        /// The no arguments constant.
        /// </summary>
        private static readonly object[] NoArgs = new object[0];

        /// <summary>
        /// The NLog logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        public InternalLogger(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Log(Logging.LogLevel level, object message, Exception exception = null)
        {
            switch (level)
            {
                case Logging.LogLevel.Fatal:
                    this.LogWithException(message, exception, this.logger.Fatal, this.logger.Fatal);
                    break;
                case Logging.LogLevel.Error:
                    this.LogWithException(message, exception, this.logger.Error, this.logger.Error);
                    break;
                case Logging.LogLevel.Warning:
                    this.LogWithException(message, exception, this.logger.Warn, this.logger.Warn);
                    break;
                case Logging.LogLevel.Info:
                    this.LogWithException(message, exception, this.logger.Info, this.logger.Info);
                    break;
                case Logging.LogLevel.Debug:
                    this.LogWithException(message, exception, this.logger.Debug, this.logger.Debug);
                    break;
                case Logging.LogLevel.Trace:
                    this.LogWithException(message, exception, this.logger.Trace, this.logger.Trace);
                    break;
                default:
                    this.LogWithException(message, exception, this.logger.Trace, this.logger.Trace);
                    break;
            }
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Log(Logging.LogLevel level, string messageFormat, params object[] args)
        {
            switch (level)
            {
                case Logging.LogLevel.Fatal:
                    this.logger.Fatal(messageFormat, args);
                    break;
                case Logging.LogLevel.Error:
                    this.logger.Error(messageFormat, args);
                    break;
                case Logging.LogLevel.Warning:
                    this.logger.Warn(messageFormat, args);
                    break;
                case Logging.LogLevel.Info:
                    this.logger.Info(messageFormat, args);
                    break;
                case Logging.LogLevel.Debug:
                    this.logger.Debug(messageFormat, args);
                    break;
                case Logging.LogLevel.Trace:
                    this.logger.Trace(messageFormat, args);
                    break;
                default:
                    this.logger.Trace(messageFormat, args);
                    break;
            }
        }

        /// <summary>
        /// Logs the message with exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="exceptionLogger">The exception logger.</param>
        /// <param name="messageLogger">The message logger.</param>
        private void LogWithException(
            object message,
            Exception exception,
            Action<Exception, string, object[]> exceptionLogger,
            Action<object> messageLogger)
        {
            if (exception != null)
            {
                exceptionLogger(exception, message.ToString(), NoArgs);
            }
            else
            {
                messageLogger(message);
            }
        }
    }
}