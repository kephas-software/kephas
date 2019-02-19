// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aggregate logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An aggregate logger.
    /// </summary>
    public class AggregateLogger : ILogger
    {
        /// <summary>
        /// The loggers.
        /// </summary>
        private readonly IEnumerable<ILogger> loggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger"/> class.
        /// </summary>
        /// <remarks>
        /// There is no requirement that the loggers contain at least one element.
        /// </remarks>
        /// <param name="loggers">A variable-length parameters list containing loggers.</param>
        public AggregateLogger(params ILogger[] loggers)
        {
            this.loggers = loggers ?? new ILogger[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger"/> class.
        /// </summary>
        /// <remarks>
        /// There is no requirement that the loggers contain at least one element.
        /// </remarks>
        /// <param name="loggers">A variable-length parameters list containing loggers.</param>
        public AggregateLogger(IEnumerable<ILogger> loggers)
        {
            this.loggers = loggers ?? new ILogger[0];
        }

        /// <summary>Logs the information at the provided level.</summary>
        /// <remarks>
        /// Note for implementors: the <paramref name="exception" /> may be <c>null</c>, so be cautious and handle this case too.
        /// For example, the <see cref="M:Kephas.Logging.LoggerExtensions.Log(Kephas.Logging.ILogger,Kephas.Logging.LogLevel,System.String,System.Object[])" /> extension method passes a <c>null</c> exception.
        /// </remarks>
        /// <param name="level">The logging level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            foreach (var logger in this.loggers)
            {
                logger.Log(level, exception, messageFormat, args);
            }
        }

        /// <summary>
        /// Indicates whether logging at the indicated level is enabled.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public bool IsEnabled(LogLevel level) => this.loggers.Any(l => l.IsEnabled(level));
    }
}