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
    public class JsonTraceWriter : ITraceWriter
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTraceWriter"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public JsonTraceWriter(ILogManager logManager)
        {
            this.logger = logManager.GetLogger(typeof(JsonSerializer));
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
            => this.logger.IsFatalEnabled() || this.logger.IsErrorEnabled()
                ? TraceLevel.Error
                : this.logger.IsWarningEnabled()
                    ? TraceLevel.Warning
                    : this.logger.IsInfoEnabled()
                        ? TraceLevel.Info
                        : this.logger.IsDebugEnabled() || this.logger.IsTraceEnabled()
                            ? TraceLevel.Verbose
                            : TraceLevel.Off;

        /// <summary>
        /// Writes the specified trace level, message and optional exception.
        /// </summary>
        /// <param name="level">The <see cref="T:System.Diagnostics.TraceLevel" /> at which to write this
        ///                     trace.</param>
        /// <param name="message">The trace message.</param>
        /// <param name="ex">The trace exception. This parameter is optional.</param>
        public void Trace(TraceLevel level, string message, Exception ex)
        {
            this.logger.Log(this.ToLogLevel(level), ex, message);
        }

        private LogLevel ToLogLevel(TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Error:
                    return LogLevel.Error;
                case TraceLevel.Warning:
                    return LogLevel.Warning;
                case TraceLevel.Info:
                    return LogLevel.Info;
                case TraceLevel.Verbose:
                    return LogLevel.Debug;
                case TraceLevel.Off:
                    return LogLevel.Trace;
                default:
                    return LogLevel.Trace;
            }
        }
    }
}
