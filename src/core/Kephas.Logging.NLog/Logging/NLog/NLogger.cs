// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   NLog logger for the <typeparamref name="TService" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.NLog
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// NLog logger for the <typeparamref name="TService"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    public class NLogger<TService> : ILogger<TService>
    {
        /// <summary>
        /// The inner logger.
        /// </summary>
        private readonly Logging.ILogger innerLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogger{TService}"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public NLogger(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.innerLogger = logManager.GetLogger(typeof(TService));
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Log(Logging.LogLevel level, object message, Exception exception = null)
        {
            this.innerLogger.Log(level, message, exception);
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Log(Logging.LogLevel level, string messageFormat, params object[] args)
        {
            this.innerLogger.Log(level, messageFormat, args);
        }
    }
}