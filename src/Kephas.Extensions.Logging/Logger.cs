// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using System;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An ASP net logger.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Kephas.Logging.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Logger(Kephas.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">Type of the state.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var kephasLogLevel = this.ToLogLevel(logLevel);
            if (logger.IsEnabled(kephasLogLevel))
            {
                logger.Log(kephasLogLevel, exception, formatter(state, exception));
            }
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.logger.IsEnabled(this.ToLogLevel(logLevel));
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
            return new LogScope();
        }

        /// <summary>
        /// Converts a logLevel to a log level.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <returns>
        /// LogLevel as a Kephas.Logging.LogLevel.
        /// </returns>
        private Kephas.Logging.LogLevel ToLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return Kephas.Logging.LogLevel.Fatal;
                case LogLevel.Error:
                    return Kephas.Logging.LogLevel.Error;
                case LogLevel.Warning:
                    return Kephas.Logging.LogLevel.Warning;
                case LogLevel.Information:
                    return Kephas.Logging.LogLevel.Info;
                case LogLevel.Debug:
                    return Kephas.Logging.LogLevel.Debug;
                case LogLevel.Trace:
                    return Kephas.Logging.LogLevel.Trace;
                default:
                    return Kephas.Logging.LogLevel.Trace;
            }
        }

        /// <summary>
        /// A log scope.
        /// </summary>
        private class LogScope : IDisposable
        {
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
            }
        }
    }
}