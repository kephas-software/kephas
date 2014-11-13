// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLogManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A log manager service which does not log anything.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A log manager service which does not log anything.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullLogManager : ILogManager
    {
        /// <summary>
        /// The null logger.
        /// </summary>
        private readonly NullLogger nullLogger = new NullLogger();

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public ILogger GetLogger(string loggerName)
        {
            return this.nullLogger;
        }

        /// <summary>
        /// A logger that does not log anything.
        /// </summary>
        private class NullLogger : ILogger
        {
            /// <summary>
            /// Logs fatal exceptions.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Fatal(object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the fatal format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void FatalFormat(string messageFormat, params object[] args)
            {
            }

            /// <summary>
            /// Logs the error.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Error(object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the error format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void ErrorFormat(string messageFormat, params object[] args)
            {
            }

            /// <summary>
            /// Logs the warning.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Warn(object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the warn format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void WarnFormat(string messageFormat, params object[] args)
            {
            }

            /// <summary>
            /// Logs the information.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Info(object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the information format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void InfoFormat(string messageFormat, params object[] args)
            {
            }

            /// <summary>
            /// Logs the debug.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Debug(object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the debug format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void DebugFormat(string messageFormat, params object[] args)
            {
            }

            /// <summary>
            /// Logs the trace.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Trace(object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the trace format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void TraceFormat(string messageFormat, params object[] args)
            {
            }
        }
    }
}