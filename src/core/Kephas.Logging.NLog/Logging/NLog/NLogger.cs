// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogger.cs" company="Quartz Software SRL">
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
    internal class NLogger : ILogger
    {
        /// <summary>
        /// The NLog logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        public NLogger(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logs fatal exceptions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception = null)
        {
            this.LogWithException(message, exception, this.logger.Fatal, this.logger.Fatal);
        }

        /// <summary>
        /// Logs the fatal format.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void FatalFormat(string messageFormat, params object[] args)
        {
            this.logger.Fatal(messageFormat, args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception = null)
        {
            this.LogWithException(message, exception, this.logger.Error, this.logger.Error);
        }

        /// <summary>
        /// Logs the error format.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void ErrorFormat(string messageFormat, params object[] args)
        {
            this.logger.Error(messageFormat, args);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception = null)
        {
            this.LogWithException(message, exception, this.logger.Warn, this.logger.Warn);
        }

        /// <summary>
        /// Logs the warn format.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void WarnFormat(string messageFormat, params object[] args)
        {
            this.logger.Warn(messageFormat, args);
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception = null)
        {
            this.LogWithException(message, exception, this.logger.Info, this.logger.Info);
        }

        /// <summary>
        /// Logs the information format.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void InfoFormat(string messageFormat, params object[] args)
        {
            this.logger.Info(messageFormat, args);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception = null)
        {
            this.LogWithException(message, exception, this.logger.Debug, this.logger.Debug);
        }

        /// <summary>
        /// Logs the debug format.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void DebugFormat(string messageFormat, params object[] args)
        {
            this.logger.Debug(messageFormat, args);
        }

        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Trace(object message, Exception exception = null)
        {
            this.LogWithException(message, exception, this.logger.Trace, this.logger.Trace);
        }

        /// <summary>
        /// Logs the trace format.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void TraceFormat(string messageFormat, params object[] args)
        {
            this.logger.Trace(messageFormat, args);
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
            Action<string, Exception> exceptionLogger,
            Action<object> messageLogger)
        {
            if (exception != null)
            {
                exceptionLogger(message.ToString(), exception);
            }
            else
            {
                messageLogger(message);
            }
        }
    }
}