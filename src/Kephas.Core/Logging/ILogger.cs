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
        /// <remarks>
        /// Note for implementors: the <paramref name="exception"/> may be <c>null</c>, so be cautious and handle this case too.
        /// For example, the <see cref="LoggerExtensions.Log"/> extension method passes a <c>null</c> exception.
        /// </remarks>
        /// <param name="level">The logging level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        void Log(LogLevel level, Exception exception, string messageFormat, params object[] args);
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
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="level">        The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">         The arguments.</param>
        public static void Log(this ILogger logger, LogLevel level, string messageFormat, params object[] args)
        {
            logger?.Log(level, null, messageFormat, args);
        }

        /// <summary>
        /// Logs fatal exceptions.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Fatal(this ILogger logger, Exception exception, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Fatal, exception, messageFormat, args);
        }

        /// <summary>
        /// Logs the fatal format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Fatal(this ILogger logger, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Fatal, messageFormat, args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(this ILogger logger, Exception exception, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Error, exception, messageFormat, args);
        }

        /// <summary>
        /// Loes the error format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(this ILogger logger, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Error, messageFormat, args);
        }

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Warn(this ILogger logger, Exception exception, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Warning, exception, messageFormat, args);
        }

        /// <summary>
        /// Logs the warn format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Warn(this ILogger logger, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Warning, messageFormat, args);
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Info(this ILogger logger, Exception exception, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Info, exception, messageFormat, args);
        }

        /// <summary>
        /// Logs the information format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Info(this ILogger logger, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Info, messageFormat, args);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Debug(this ILogger logger, Exception exception, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Debug, exception, messageFormat, args);
        }

        /// <summary>
        /// Logs the debug format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Debug(this ILogger logger, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Debug, messageFormat, args);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void Trace(this ILogger logger, Exception exception, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Trace, exception, messageFormat, args);
        }

        /// <summary>
        /// Logs the debug format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void TraceFormat(this ILogger logger, string messageFormat, params object[] args)
        {
            logger?.Log(LogLevel.Trace, messageFormat, args);
        }
    }
}