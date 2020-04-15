// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   NLog logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using System;

    using global::NLog;

    /// <summary>
    /// The NLog logger adapter.
    /// </summary>
    public class NLogger : Logging.ILogger
    {
        /// <summary>
        /// The NLog logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogger"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger.</param>
        protected internal NLogger(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Indicates whether logging at the indicated level is enabled.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <returns>
        /// <c>true</c> if enabled, <c>false</c> if not.
        /// </returns>
        public bool IsEnabled(Logging.LogLevel level)
        {
            return this.logger.IsEnabled(NLogManager.ToNLogLevel(level));
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">        The logging level.</param>
        /// <param name="exception">    The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">         A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public bool Log(Logging.LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            if (exception == null)
            {
                this.logger.Log(NLogManager.ToNLogLevel(level), messageFormat, args);
            }
            else
            {
                this.logger.Log(NLogManager.ToNLogLevel(level), exception, messageFormat, args);
            }

            return true;
        }
    }
}