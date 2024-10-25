// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogLevelSwitch.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Logging;

/// <summary>
/// Service for adjusting the log level.
/// </summary>
public interface ILogLevelSwitch
{
    /// <summary>
    /// Sets the log level to the provided value.
    /// </summary>
    /// <param name="logLevel">The new log level.</param>
    void SetLogLevel(LogLevel logLevel);

    /// <summary>
    /// Gets the current log level.
    /// </summary>
    /// <returns>The current log level.</returns>
    LogLevel GetLogLevel();

    /// <summary>
    /// Gets the default log level.
    /// </summary>
    /// <returns>The default log level.</returns>
    LogLevel GetDefaultLogLevel();
}