// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A log4net logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Log4Net
{
    using System;

    using log4net;

    /// <summary>
    /// A log4net logger.
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        /// <summary>
        /// The log4net logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        protected internal Log4NetLogger(ILog logger)
        {
            this.logger = logger;
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
            if (exception == null)
            {
                this.Log(level, messageFormat, args);
                return;
            }

            var message = args == null || args.Length == 0 ? messageFormat : string.Format(messageFormat, args);

            switch (level)
            {
                case LogLevel.Fatal:
                    this.logger.Fatal(message, exception);
                    break;
                case LogLevel.Error:
                    this.logger.Error(message, exception);
                    break;
                case LogLevel.Warning:
                    this.logger.Warn(message, exception);
                    break;
                case LogLevel.Info:
                    this.logger.Info(message, exception);
                    break;
                case LogLevel.Debug:
                    this.logger.Debug(message, exception);
                    break;
                case LogLevel.Trace:
                    this.logger.Debug(message, exception);
                    break;
                default:
                    this.logger.Debug(message, exception);
                    break;
            }
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Log(LogLevel level, string messageFormat, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    this.logger.FatalFormat(messageFormat, args);
                    break;
                case LogLevel.Error:
                    this.logger.ErrorFormat(messageFormat, args);
                    break;
                case LogLevel.Warning:
                    this.logger.WarnFormat(messageFormat, args);
                    break;
                case LogLevel.Info:
                    this.logger.InfoFormat(messageFormat, args);
                    break;
                case LogLevel.Debug:
                    this.logger.DebugFormat(messageFormat, args);
                    break;
                case LogLevel.Trace:
                    this.logger.DebugFormat(messageFormat, args);
                    break;
                default:
                    this.logger.DebugFormat(messageFormat, args);
                    break;
            }
        }
    }
}