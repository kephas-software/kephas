// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the log class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Logging
{
    using System;

    using Kephas.Logging;

    using global::ServiceStack.Logging;

    /// <summary>
    /// The logger adapter for ServiceStack.
    /// </summary>
    public class Log : ILog
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Log(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets a value indicating whether the debug is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the debug is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled => this.logger.IsDebugEnabled();

        /// <summary>
        /// Logs a Debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            this.logger.Debug(message?.ToString());
        }

        /// <summary>
        /// Logs a Debug message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
            this.logger.Debug(exception, message?.ToString());
        }

        /// <summary>
        /// Logs a Debug format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            this.logger.Debug(format, args);
        }

        /// <summary>
        /// Logs an Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
            this.logger.Error(message?.ToString());
        }

        /// <summary>
        /// Logs an Error message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
            this.logger.Error(exception, message?.ToString());
        }

        /// <summary>
        /// Logs an Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            this.logger.Error(format, args);
        }

        /// <summary>
        /// Logs a Fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
            this.logger.Fatal(message?.ToString());
        }

        /// <summary>
        /// Logs a Fatal message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
            this.logger.Fatal(exception, message?.ToString());
        }

        /// <summary>
        /// Logs a Error format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            this.logger.Fatal(format, args);
        }

        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
            this.logger.Info(message?.ToString());
        }

        /// <summary>
        /// Logs an Info message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
            this.logger.Info(exception, message?.ToString());
        }

        /// <summary>
        /// Logs an Info format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            this.logger.Info(format, args);
        }

        /// <summary>
        /// Logs a Warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
            this.logger.Warn(message?.ToString());
        }

        /// <summary>
        /// Logs a Warning message and exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
            this.logger.Warn(exception, message?.ToString());
        }

        /// <summary>
        /// Logs a Warning format message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            this.logger.Warn(format, args);
        }
    }
}