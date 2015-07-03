// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Logger abstract interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Enumerates the logging levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Fatal errors.
        /// </summary>
        Fatal,

        /// <summary>
        /// Common errors.
        /// </summary>
        Error,

        /// <summary>
        /// Warning information.
        /// </summary>
        Warning,

        /// <summary>
        /// Common information.
        /// </summary>
        Info,

        /// <summary>
        /// Debugging information.
        /// </summary>
        Debug,

        /// <summary>
        /// Tracing information.
        /// </summary>
        Trace,
    }

    /// <summary>
    /// Logger abstract interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Log(LogLevel level, object message, Exception exception = null);

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        void Log(LogLevel level, string messageFormat, params object[] args);
    }

    /// <summary>
    /// Defines a service contract for a logger associated to a specific service.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    [SharedAppServiceContract(AsOpenGeneric = true)]
    public interface ILogger<TService> : ILogger
    {
    }

    /// <summary>
    /// Extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs fatal exceptions.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Fatal(this ILogger logger, object message, Exception exception = null)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Logs the fatal format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void FatalFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Fatal, messageFormat, args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Error(this ILogger logger, object message, Exception exception = null)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Loes the error format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void ErrorFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Error, messageFormat, args);
        }

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Warn(this ILogger logger, object message, Exception exception = null)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Warning, message, exception);
        }

        /// <summary>
        /// Logs the warn format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void WarnFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Warning, messageFormat, args);
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Info(this ILogger logger, object message, Exception exception = null)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Info, message, exception);
        }

        /// <summary>
        /// Logs the information format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void InfoFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Info, messageFormat, args);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Debug(this ILogger logger, object message, Exception exception = null)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Logs the debug format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void DebugFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Debug, messageFormat, args);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Trace(this ILogger logger, object message, Exception exception = null)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Trace, message, exception);
        }

        /// <summary>
        /// Logs the debug format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void TraceFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Trace, messageFormat, args);
        }
    }
}