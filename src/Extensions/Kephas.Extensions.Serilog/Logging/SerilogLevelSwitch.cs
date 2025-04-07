// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogLevelSwitch.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Kephas.Extensions.Logging;

/// <summary>
/// The Serilog implementation of the <see cref="ILogLevelSwitch"/>.
/// </summary>
/// <param name="configuration">The configuration.</param>
public class SerilogLevelSwitch(IConfiguration configuration) : ISerilogLevelSwitch
{
    private const string SerilogMinimumLevelSection = "Serilog:MinimumLevel";
    private const string SerilogMinimumLevelDefaultSection = "Serilog:MinimumLevel:Default";

    /// <summary>
    /// Gets the switch for the logging level.
    /// </summary>
    public LoggingLevelSwitch LevelSwitch { get; } = new(GetDefaultSerilogLevel(configuration));

    /// <summary>
    /// Sets the log level to the provided value.
    /// </summary>
    /// <param name="logLevel">The new log level.</param>
    public void SetLogLevel(LogLevel logLevel)
    {
        LevelSwitch.MinimumLevel = logLevel switch
        {
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Verbose,
        };
    }

    /// <summary>
    /// Gets the current log level.
    /// </summary>
    /// <returns>The current log level.</returns>
    public LogLevel GetLogLevel() =>
        ToLogLevel(LevelSwitch.MinimumLevel);

    /// <summary>
    /// Gets the default log level.
    /// </summary>
    /// <returns>The default log level.</returns>
    public LogLevel GetDefaultLogLevel() =>
        ToLogLevel(GetDefaultSerilogLevel(configuration));

    private static LogEventLevel GetDefaultSerilogLevel(IConfiguration configuration)
    {
        var logLevelString = configuration[SerilogMinimumLevelDefaultSection]
                             ?? configuration[SerilogMinimumLevelSection];
        return string.IsNullOrEmpty(logLevelString) ? default : Enum.Parse<LogEventLevel>(logLevelString);
    }

    private static LogLevel ToLogLevel(LogEventLevel logEventLevel) =>
        logEventLevel switch
        {
            LogEventLevel.Fatal => LogLevel.Critical,
            LogEventLevel.Error => LogLevel.Error,
            LogEventLevel.Warning => LogLevel.Warning,
            LogEventLevel.Information => LogLevel.Information,
            LogEventLevel.Debug => LogLevel.Debug,
            _ => LogLevel.Trace,
        };
}