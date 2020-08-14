// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using global::Serilog.Events;

    /// <summary>
    /// Helper class for Serilog adapters.
    /// </summary>
    public static class SerilogHelper
    {

        /// <summary>
        /// Converts the log level to Serilog log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The Serilog log level.</returns>
        public static LogEventLevel ToLogEventLevel(this LogLevel level)
        {
            return level switch
            {
                LogLevel.Fatal => LogEventLevel.Fatal,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Info => LogEventLevel.Information,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Trace => LogEventLevel.Verbose,
                _ => LogEventLevel.Verbose
            };
        }

        /// <summary>
        /// Converts the Serilog log level to log level.
        /// </summary>
        /// <param name="level">The Serilog log level.</param>
        /// <returns>The log level.</returns>
        public static LogLevel ToLogLevel(this LogEventLevel level)
        {
            return level switch
            {
                LogEventLevel.Fatal => LogLevel.Fatal,
                LogEventLevel.Error => LogLevel.Error,
                LogEventLevel.Warning => LogLevel.Warning,
                LogEventLevel.Information => LogLevel.Info,
                LogEventLevel.Debug => LogLevel.Debug,
                LogEventLevel.Verbose => LogLevel.Trace,
                _ => LogLevel.Trace
            };
        }
    }
}
