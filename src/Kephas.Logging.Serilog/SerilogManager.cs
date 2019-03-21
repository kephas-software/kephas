// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serilog manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using System.Collections.Concurrent;

    /// <summary>
    /// The Serilog log manager.
    /// </summary>
    public class SerilogManager : ILogManager
    {
        /// <summary>
        /// The loggers dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        public ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, this.CreateLogger);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual ILogger CreateLogger(string loggerName)
        {
            var logger = new SerilogLogger(loggerName);
            return logger;
        }
    }
}