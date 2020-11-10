// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTraceWriter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON trace writer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Logging
{
    using System;
    using System.Diagnostics;

    using Kephas.Logging;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A JSON trace writer.
    /// </summary>
    public class JsonTraceWriter : Loggable, ITraceWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTraceWriter"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public JsonTraceWriter(ILogManager? logManager = null)
            : base(logManager, typeof(JsonSerializer))
        {
        }

        /// <summary>
        /// Gets the <see cref="T:System.Diagnostics.TraceLevel" /> that will be used to filter the trace
        /// messages passed to the writer. For example a filter level of
        /// <see cref="F:System.Diagnostics.TraceLevel.Info" /> will exclude
        /// <see cref="F:System.Diagnostics.TraceLevel.Verbose" /> messages and include
        /// <see cref="F:System.Diagnostics.TraceLevel.Info" />,
        /// <see cref="F:System.Diagnostics.TraceLevel.Warning" /> and
        /// <see cref="F:System.Diagnostics.TraceLevel.Error" /> messages.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Diagnostics.TraceLevel" /> that will be used to filter the trace
        /// messages passed to the writer.
        /// </value>
        public TraceLevel LevelFilter
            => this.Logger.IsFatalEnabled() || this.Logger.IsErrorEnabled()
                ? TraceLevel.Error
                : this.Logger.IsWarningEnabled()
                    ? TraceLevel.Warning
                    : this.Logger.IsInfoEnabled()
                        ? TraceLevel.Info
                        : this.Logger.IsDebugEnabled() || this.Logger.IsTraceEnabled()
                            ? TraceLevel.Verbose
                            : TraceLevel.Off;

        /// <summary>
        /// Writes the specified trace level, message and optional exception.
        /// </summary>
        /// <param name="level">The <see cref="T:System.Diagnostics.TraceLevel" /> at which to write this
        ///                     trace.</param>
        /// <param name="message">The trace message.</param>
        /// <param name="ex">The trace exception. This parameter is optional.</param>
        public void Trace(TraceLevel level, string message, Exception ex) =>
            this.Logger.Log(this.ToLogLevel(level), ex, message);

        private LogLevel ToLogLevel(TraceLevel traceLevel) =>
            traceLevel switch
            {
                TraceLevel.Error => LogLevel.Error,
                TraceLevel.Warning => LogLevel.Warning,
                TraceLevel.Info => LogLevel.Info,
                TraceLevel.Verbose => LogLevel.Debug,
                TraceLevel.Off => LogLevel.Trace,
                _ => LogLevel.Trace
            };
    }
}
