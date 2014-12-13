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
            /// Logs the information at the provided level.
            /// </summary>
            /// <param name="level">The logging level.</param>
            /// <param name="message">The message.</param>
            /// <param name="exception">The exception.</param>
            public void Log(LogLevel level, object message, Exception exception = null)
            {
            }

            /// <summary>
            /// Logs the information at the provided level.
            /// </summary>
            /// <param name="level">The logging level.</param>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            public void Log(LogLevel level, string messageFormat, params object[] args)
            {
            }
        }
    }
}