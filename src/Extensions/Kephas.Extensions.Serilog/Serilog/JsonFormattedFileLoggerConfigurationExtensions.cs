// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonFormattedFileLoggerConfigurationExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Logging.Formatting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.File;

namespace Kephas.Serilog;

/// <summary>
/// Logger configuration extensions using
/// </summary>
public static class JsonFormattedFileLoggerConfigurationExtensions
{
    const int DefaultRetainedFileCountLimit = 31; // A long month of logs
    const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024; // 1GB

    /// <summary>
        /// Write log events to the specified JSON formatted file.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="path">Path to the file.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="fileSizeLimitBytes">The approximate maximum size, in bytes, to which a log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB. To avoid writing partial events, the last event within the limit
        /// will be written in full even if it exceeds the limit.</param>
        /// <param name="buffered">Indicates if flushing to the output file can be buffered or not. The default
        /// is false.</param>
        /// <param name="shared">Allow the log file to be shared by multiple processes. The default is false.</param>
        /// <param name="flushToDiskInterval">If provided, a full disk flush will be performed periodically at the specified interval.</param>
        /// <param name="rollingInterval">The interval at which logging will roll over to a new file.</param>
        /// <param name="rollOnFileSizeLimit">If <code>true</code>, a new file will be created when the file size limit is reached. Filenames
        /// will have a number appended in the format <code>_NNN</code>, with the first filename given no number.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <param name="encoding">Character encoding used to write the text file. The default is UTF-8 without BOM.</param>
        /// <param name="hooks">Optionally enables hooking into log file lifecycle events.</param>
        /// <param name="retainedFileTimeLimit">The maximum time after the end of an interval that a rolling log file will be retained.
        /// Must be greater than or equal to <see cref="TimeSpan.Zero"/>.
        /// Ignored if <paramref see="rollingInterval"/> is <see cref="RollingInterval.Infinite"/>.
        /// The default is to retain files indefinitely.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <code>null</code></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="PathTooLongException">When <paramref name="path"/> is too long</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission to access the <paramref name="path"/></exception>
        /// <exception cref="ArgumentException">Invalid <paramref name="path"/></exception>
        public static LoggerConfiguration JsonFormattedFile(
            this LoggerSinkConfiguration sinkConfiguration,
            string path,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            LoggingLevelSwitch? levelSwitch = null,
            bool buffered = false,
            bool shared = true,
            TimeSpan? flushToDiskInterval = null,
            RollingInterval rollingInterval = RollingInterval.Day,
            bool rollOnFileSizeLimit = true,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            Encoding? encoding = null,
            FileLifecycleHooks? hooks = null,
            TimeSpan? retainedFileTimeLimit = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (path == null) throw new ArgumentNullException(nameof(path));

            ITextFormatter formatter = new JsonFormatter();
            flushToDiskInterval ??= TimeSpan.FromSeconds(1);

            return sinkConfiguration.File(
                formatter,
                path,
                restrictedToMinimumLevel,
                fileSizeLimitBytes,
                levelSwitch,
                buffered,
                shared,
                flushToDiskInterval,
                rollingInterval,
                rollOnFileSizeLimit,
                retainedFileCountLimit,
                encoding,
                hooks,
                retainedFileTimeLimit);
        }
}
