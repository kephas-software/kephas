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
    public class NLogger : Logging.ILogger
    {
        /// <summary>
        /// The NLog logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        protected internal NLogger(Logger logger)
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
        public bool IsEnabled(Logging.LogLevel level)
        {
            if (this.logger.IsTraceEnabled)
            {
                return level <= Logging.LogLevel.Trace;
            }

            if (this.logger.IsDebugEnabled)
            {
                return level <= Logging.LogLevel.Debug;
            }

            if (this.logger.IsInfoEnabled)
            {
                return level <= Logging.LogLevel.Info;
            }

            if (this.logger.IsWarnEnabled)
            {
                return level <= Logging.LogLevel.Warning;
            }

            if (this.logger.IsErrorEnabled)
            {
                return level <= Logging.LogLevel.Error;
            }

            if (this.logger.IsFatalEnabled)
            {
                return level <= Logging.LogLevel.Fatal;
            }

            return false;
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">        The logging level.</param>
        /// <param name="exception">    The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">         A variable-length parameters list containing arguments.</param>
        public void Log(Logging.LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            if (exception == null)
            {
                this.Log(level, messageFormat, args);
                return;
            }

            switch (level)
            {
                case Logging.LogLevel.Fatal:
                    this.logger.Fatal(exception, messageFormat, args);
                    break;
                case Logging.LogLevel.Error:
                    this.logger.Error(exception, messageFormat, args);
                    break;
                case Logging.LogLevel.Warning:
                    this.logger.Warn(exception, messageFormat, args);
                    break;
                case Logging.LogLevel.Info:
                    this.logger.Info(exception, messageFormat, args);
                    break;
                case Logging.LogLevel.Debug:
                    this.logger.Debug(exception, messageFormat, args);
                    break;
                case Logging.LogLevel.Trace:
                    this.logger.Trace(exception, messageFormat, args);
                    break;
                default:
                    this.logger.Trace(exception, messageFormat, args);
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
    }
}