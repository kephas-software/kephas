// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Logging;

/// <summary>
/// Extension methods for <see cref="ILogger"/>.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Gets a value indicating whether the severity level indicates an error.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>
    /// True if error, false if not.
    /// </returns>
    public static bool IsError(this LogLevel severity) => severity <= LogLevel.Error;

    /// <summary>
    /// Gets the log level for an object, typically an <see cref="Exception"/> or an error object.
    /// This is calculated to be <see cref="LogLevel.Error"/> if the obj does not implement <see cref="IHasLogLevel"/>,
    /// otherwise the level indicated by the severity level.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// The log level.
    /// </returns>
    public static LogLevel GetLogLevel(this object obj)
    {
        _ = obj ?? throw new ArgumentNullException(nameof(obj));

        return obj switch
        {
            IHasLogLevel hasLogLevel => hasLogLevel.LogLevel,
            Exception => LogLevel.Error,
            _ => LogLevel.Information,
        };
    }

    /// <summary>
    /// Indicates whether logging is enabled at the <see cref="LogLevel.Critical"/> level.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <returns>
    /// <c>true</c> if enabled, <c>false</c> if not.
    /// </returns>
    public static bool IsCriticalEnabled(this ILogger? logger)
    {
        return logger?.IsEnabled(LogLevel.Critical) ?? false;
    }

    /// <summary>
    /// Indicates whether logging is enabled at the <see cref="LogLevel.Critical"/> level.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <returns>
    /// <c>true</c> if enabled, <c>false</c> if not.
    /// </returns>
    public static bool IsFatalEnabled(this ILogger? logger)
    {
        return logger?.IsEnabled(LogLevel.Critical) ?? false;
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
    /// Indicates whether logging is enabled at the <see cref="LogLevel.Information"/> level.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <returns>
    /// <c>true</c> if enabled, <c>false</c> if not.
    /// </returns>
    public static bool IsInformationEnabled(this ILogger? logger)
    {
        return logger?.IsEnabled(LogLevel.Information) ?? false;
    }

    /// <summary>
    /// Indicates whether logging is enabled at the <see cref="LogLevel.Information"/> level.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <returns>
    /// <c>true</c> if enabled, <c>false</c> if not.
    /// </returns>
    public static bool IsInfoEnabled(this ILogger? logger)
    {
        return logger?.IsEnabled(LogLevel.Information) ?? false;
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
