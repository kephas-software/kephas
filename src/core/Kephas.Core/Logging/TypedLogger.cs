// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypedLogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Typed logger for the <typeparamref name="TService" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// NLog logger for the <typeparamref name="TService"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    [OverridePriority(Priority.Low)]
    public class TypedLogger<TService> : ILogger<TService>
    {
        /// <summary>
        /// The inner logger.
        /// </summary>
        private readonly ILogger innerLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedLogger{TService}"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public TypedLogger(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.innerLogger = logManager.GetLogger(typeof(TService));
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">        The logging level.</param>
        /// <param name="exception">    The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">         The arguments.</param>
        public void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            this.innerLogger.Log(level, exception, messageFormat, args);
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">        The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">         The arguments.</param>
        public void Log(LogLevel level, string messageFormat, params object[] args)
        {
            this.innerLogger.Log(level, messageFormat, args);
        }
    }
}