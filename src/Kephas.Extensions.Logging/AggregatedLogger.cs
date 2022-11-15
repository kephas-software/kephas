// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatedLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aggregated logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Microsoft.Extensions.Logging;

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    /// <summary>
    /// An aggregated logger.
    /// </summary>
    internal class AggregatedLogger : ILogger
    {
        private readonly IEnumerable<ILogger> loggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedLogger"/> class.
        /// </summary>
        /// <param name="loggers">The loggers.</param>
        public AggregatedLogger(IList<ILogger> loggers)
        {
            this.loggers = loggers;
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">Type of the state.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>
        /// An IDisposable that ends the logical operation scope on dispose.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return new AggregatedScope(this.loggers.Select(l => l.BeginScope(state)).ToList());
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>
        /// <c>true</c> if enabled.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.loggers.Any(l => l.IsEnabled(logLevel));
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">Type of the state.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the
        ///                         <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.loggers.ForEach(l => l.Log(logLevel, eventId, state, exception, formatter));
        }

        private class AggregatedScope : IDisposable
        {
            private readonly IList<IDisposable> scopes;

            public AggregatedScope(IList<IDisposable> scopes)
            {
                this.scopes = scopes;
            }

            public void Dispose()
            {
                this.scopes.ForEach(s => s.Dispose());
            }
        }
    }
}
