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
    using log4net;

    /// <summary>
    /// A log4net logger.
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        private readonly ILog logger;
        private readonly StructuredLogEntryProvider entryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        protected internal Log4NetLogger(ILog logger)
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
            if (this.logger.IsDebugEnabled)
            {
                return level <= LogLevel.Trace;
            }

            if (this.logger.IsInfoEnabled)
            {
                return level <= LogLevel.Info;
            }

            if (this.logger.IsWarnEnabled)
            {
                return level <= LogLevel.Warning;
            }

            if (this.logger.IsErrorEnabled)
            {
                return level <= LogLevel.Error;
            }

            if (this.logger.IsFatalEnabled)
            {
                return level <= LogLevel.Fatal;
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
        public void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            if (exception == null)
            {
                this.Log(level, messageFormat, args);
                return;
            }

            var (message, positionalArgs, _) = this.entryProvider.GetLogEntry(messageFormat, args);
            message = positionalArgs == null || positionalArgs.Length == 0 ? message : string.Format(message, positionalArgs);

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
            var (message, positionalArgs, namedArgs) = this.entryProvider.GetLogEntry(messageFormat, args);

            switch (level)
            {
                case LogLevel.Fatal:
                    if (positionalArgs == null)
                    {
                        this.logger.Fatal(message);
                    }
                    else
                    {
                        this.logger.FatalFormat(messageFormat, positionalArgs);
                    }

                    break;
                case LogLevel.Error:
                    if (positionalArgs == null)
                    {
                        this.logger.Error(message);
                    }
                    else
                    {
                        this.logger.ErrorFormat(messageFormat, positionalArgs);
                    }

                    break;
                case LogLevel.Warning:
                    if (positionalArgs == null)
                    {
                        this.logger.Warn(message);
                    }
                    else
                    {
                        this.logger.WarnFormat(messageFormat, positionalArgs);
                    }

                    break;
                case LogLevel.Info:
                    if (positionalArgs == null)
                    {
                        this.logger.Info(message);
                    }
                    else
                    {
                        this.logger.InfoFormat(messageFormat, positionalArgs);
                    }

                    break;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    if (positionalArgs == null)
                    {
                        this.logger.Debug(message);
                    }
                    else
                    {
                        this.logger.DebugFormat(messageFormat, positionalArgs);
                    }

                    break;
                default:
                    if (positionalArgs == null)
                    {
                        this.logger.Debug(message);
                    }
                    else
                    {
                        this.logger.DebugFormat(messageFormat, positionalArgs);
                    }

                    break;
            }
        }
    }
}