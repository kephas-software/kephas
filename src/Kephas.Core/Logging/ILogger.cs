// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Logger abstract interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;
    using System.Linq;

    using Kephas.ExceptionHandling;
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
        /// For example, the <see cref="LoggerExtensions.Log(ILogger, LogLevel, string, object?[])"/> extension method passes a <c>null</c> exception.
        /// </remarks>
        /// <param name="level">The logging level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments for the message format.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        bool Log(LogLevel level, Exception? exception, string? messageFormat, params object?[] args);

        /// <summary>
        /// Indicates whether logging at the indicated level is enabled.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        bool IsEnabled(LogLevel level);
    }

    /// <summary>
    /// Defines a service contract for a logger associated to a specific service.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    [SingletonAppServiceContract(AsOpenGeneric = true)]
    public interface ILogger<TService> : ILogger
    {
    }

    /// <summary>
    /// Extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Merges the loggers into one aggregate.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggers">A variable-length parameters list containing loggers.</param>
        /// <returns>
        /// An aggregated logger.
        /// </returns>
        public static ILogger? Merge(this ILogger? logger, params ILogger?[]? loggers)
        {
            var validLoggers = loggers?.Where(l => l != null).ToList();
            if (validLoggers == null || validLoggers.Count == 0)
            {
                return logger;
            }

            if (logger == null)
            {
                return validLoggers.Count == 1 ? validLoggers[0] : new AggregateLogger(validLoggers!);
            }

            validLoggers.Add(logger);
            return new AggregateLogger(validLoggers!);
        }

        /// <summary>
        /// Gets the log level for an exception.
        /// This is calculated to be <see cref="LogLevel.Error"/> if the exception does not implement <see cref="ISeverityQualifiedNotification"/>,
        /// otherwise the level indicated by the severity level.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// The log level.
        /// </returns>
        public static LogLevel GetLogLevel(this Exception? exception)
        {
            if (exception is ISeverityQualifiedNotification severityQualifiedException)
            {
                return (LogLevel)severityQualifiedException.Severity;
            }

            return LogLevel.Error;
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="level">The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Log(this ILogger? logger, LogLevel level, string messageFormat, params object?[] args)
        {
            return logger?.Log(level, null, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception at the log level indicated be the exception's severity level.
        /// This is calculated to be <see cref="SeverityLevel.Error"/> if the exception does not implement <see cref="ISeverityQualifiedNotification"/>,
        /// otherwise the level indicated by the severity level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Log(this ILogger? logger, Exception exception)
        {
            var logLevel = GetLogLevel(exception);
            return logger?.Log(logLevel, exception, exception.Message) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at the log level indicated be the exception's severity level.
        /// This is calculated to be <see cref="SeverityLevel.Error"/> if the exception does not implement <see cref="ISeverityQualifiedNotification"/>,
        /// otherwise the level indicated by the severity level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Log(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            var logLevel = GetLogLevel(exception);
            return logger?.Log(logLevel, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at <see cref="LogLevel.Fatal"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Fatal(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Fatal, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs a formatted message at <see cref="LogLevel.Fatal"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Fatal(this ILogger? logger, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Fatal, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at <see cref="LogLevel.Error"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Error(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Error, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs a formatted message at <see cref="LogLevel.Error"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Error(this ILogger? logger, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Error, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at <see cref="LogLevel.Warning"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Warn(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Warning, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs a formatted message at <see cref="LogLevel.Warning"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Warn(this ILogger? logger, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Warning, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Info(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Info, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs a formatted message at <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Info(this ILogger? logger, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Info, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Debug(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Debug, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs a formatted message at <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Debug(this ILogger? logger, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Debug, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs the exception with a formatted message at <see cref="LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Trace(this ILogger? logger, Exception? exception, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Trace, exception, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Logs a formatted message at <see cref="LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public static bool Trace(this ILogger? logger, string messageFormat, params object?[] args)
        {
            return logger?.Log(LogLevel.Trace, messageFormat, args) ?? false;
        }

        /// <summary>
        /// Indicates whether logging is enabled at the <see cref="LogLevel.Fatal"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public static bool IsFatalEnabled(this ILogger? logger)
        {
            return logger?.IsEnabled(LogLevel.Fatal) ?? false;
        }

        /// <summary>
        /// Indicates whether logging is enabled at the <see cref="LogLevel.Error"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public static bool IsErrorEnabled(this ILogger? logger)
        {
            return logger?.IsEnabled(LogLevel.Error) ?? false;
        }

        /// <summary>
        /// Indicates whether logging is enabled at the <see cref="LogLevel.Warning"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public static bool IsWarningEnabled(this ILogger? logger)
        {
            return logger?.IsEnabled(LogLevel.Warning) ?? false;
        }

        /// <summary>
        /// Indicates whether logging is enabled at the <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public static bool IsInfoEnabled(this ILogger? logger)
        {
            return logger?.IsEnabled(LogLevel.Info) ?? false;
        }

        /// <summary>
        /// Indicates whether logging is enabled at the <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public static bool IsDebugEnabled(this ILogger? logger)
        {
            return logger?.IsEnabled(LogLevel.Debug) ?? false;
        }

        /// <summary>
        /// Indicates whether logging is enabled at the <see cref="LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public static bool IsTraceEnabled(this ILogger? logger)
        {
            return logger?.IsEnabled(LogLevel.Trace) ?? false;
        }
    }
}