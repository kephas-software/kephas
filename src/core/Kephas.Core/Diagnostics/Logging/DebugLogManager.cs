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
            /// Logs the information at the provided level.
            /// </summary>
            /// <param name="level">The logging level.</param>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Log(LogLevel level, object message, Exception exception = null)
            {
                this.LogCore(level.ToString(), message, exception);
            }

            /// <summary>
            /// Logs the information at the provided level.
            /// </summary>
            /// <param name="level">The logging level.</param>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void Log(LogLevel level, string messageFormat, params object[] args)
            {
                this.LogCore(level.ToString(), string.Format(messageFormat, args));
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