// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Log manager for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Logging
{
    using System;
    using System.Collections.Concurrent;

    using Kephas.Logging;

    /// <summary>
    /// Log manager for debugging.
    /// </summary>
    public class DebugLogManager : ILogManager
    {
        /// <summary>
        /// The cached loggers.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, name => new DebugLogger(name));
        }

        /// <summary>
        /// The debug logger.
        /// </summary>
        internal class DebugLogger : ILogger
        {
            /// <summary>
            /// The name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugLogger"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            public DebugLogger(string name)
            {
                this.name = name;
            }

            /// <summary>
            /// Logs fatal exceptions.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Fatal(object message, Exception exception = null)
            {
                this.LogCore("FATAL", message, exception);
            }

            /// <summary>
            /// Logs the fatal format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void FatalFormat(string messageFormat, params object[] args)
            {
                this.LogCore("FATAL", string.Format(messageFormat, args));
            }

            /// <summary>
            /// Logs the error.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Error(object message, Exception exception = null)
            {
                this.LogCore("ERROR", message, exception);
            }

            /// <summary>
            /// Logs the error format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void ErrorFormat(string messageFormat, params object[] args)
            {
                this.LogCore("ERROR", string.Format(messageFormat, args));
            }

            /// <summary>
            /// Logs the warning.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Warn(object message, Exception exception = null)
            {
                this.LogCore("WARN", message, exception);
            }

            /// <summary>
            /// Logs the warn format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void WarnFormat(string messageFormat, params object[] args)
            {
                this.LogCore("WARN", string.Format(messageFormat, args));
            }

            /// <summary>
            /// Logs the information.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Info(object message, Exception exception = null)
            {
                this.LogCore("INFO", message, exception);
            }

            /// <summary>
            /// Logs the information format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void InfoFormat(string messageFormat, params object[] args)
            {
                this.LogCore("INFO", string.Format(messageFormat, args));
            }

            /// <summary>
            /// Logs the debug.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Debug(object message, Exception exception = null)
            {
                this.LogCore("DEBUG", message, exception);
            }

            /// <summary>
            /// Logs the debug format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void DebugFormat(string messageFormat, params object[] args)
            {
                this.LogCore("DEBUG", string.Format(messageFormat, args));
            }

            /// <summary>
            /// Logs the trace.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Trace(object message, Exception exception = null)
            {
                this.LogCore("TRACE", message, exception);
            }

            /// <summary>
            /// Logs the trace format.
            /// </summary>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void TraceFormat(string messageFormat, params object[] args)
            {
                this.LogCore("TRACE", string.Format(messageFormat, args));
            }

            /// <summary>
            /// Logs the message.
            /// </summary>
            /// <param name="level">The level.</param>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            private void LogCore(string level, object message, Exception exception = null)
            {
                System.Diagnostics.Debug.WriteLine("{0}: {1}: {2} {3}", this.name, level, message, exception);
            }
        }
    }
}