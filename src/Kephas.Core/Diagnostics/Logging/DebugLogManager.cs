// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Log manager for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;

    using Kephas.Logging;

    /// <summary>
    /// Log manager for debugging.
    /// </summary>
    public class DebugLogManager : ILogManager
    {
        /// <summary>
        /// The log callback.
        /// </summary>
        private readonly Action<string, string, object, object?[], Exception?>? logCallback;

        /// <summary>
        /// The cached loggers.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogManager"/> class.
        /// </summary>
        /// <param name="logCallback">optional. The log callback.</param>
        public DebugLogManager(Action<string, string, object, object?[], Exception?>? logCallback = null)
        {
            this.logCallback = logCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogManager"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        public DebugLogManager(StringBuilder stringBuilder)
            : this(
                (logger, level, message, args, exception)
                    => stringBuilder
                        .AppendFormat(
                            "[{0}] [{1}] {2}{3}. {4}",
                            logger,
                            level,
                            message,
                            args?.Length > 0 ? " (" + string.Join(", ", args) + ")" : null,
                            exception)
                        .AppendLine())
        {
        }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, name => new DebugLogger(name, this.logCallback, () => this.MinimumLevel));
        }

        /// <summary>
        /// The debug logger.
        /// </summary>
        internal class DebugLogger : ILogger
        {
            private readonly string name;
            private readonly Action<string, string, object, object?[], Exception?>? logCallback;
            private readonly Func<LogLevel> logLevelGetter;

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugLogger"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="logCallback">The log callback.</param>
            /// <param name="logLevelGetter">Getter function for the log level.</param>
            public DebugLogger(string name, Action<string, string, object, object?[], Exception?>? logCallback, Func<LogLevel> logLevelGetter)
            {
                this.name = name;
                this.logCallback = logCallback;
                this.logLevelGetter = logLevelGetter;
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
                return level <= this.logLevelGetter();
            }

            /// <summary>
            /// Logs the information at the provided level.
            /// </summary>
            /// <param name="level">The logging level.</param>
            /// <param name="exception">The exception.</param>
            /// <param name="messageFormat">The message format.</param>
            /// <param name="args">The arguments.</param>
            /// <returns>
            /// True if the log operation succeeded, false if it failed.
            /// </returns>
            public bool Log(LogLevel level, Exception? exception, string? messageFormat, params object?[] args)
            {
                return this.LogCore(level.ToString(), messageFormat, args, exception);
            }

            private bool LogCore(string level, object message, object?[] args, Exception? exception = null)
            {
                if (this.logCallback == null)
                {
                    System.Diagnostics.Debug.WriteLine("{0}: {1}: {2} ({3}) {4}", this.name, level, message, string.Join(", ", args), exception);
                }
                else
                {
                    this.logCallback(this.name, level, message, args, exception);
                }

                return true;
            }
        }
    }
}