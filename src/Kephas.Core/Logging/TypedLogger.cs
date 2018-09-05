// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypedLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Typed logger for the <typeparamref name="TService" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Diagnostics.Contracts;
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
            Requires.NotNull(logManager, nameof(logManager));

            this.innerLogger = logManager.GetLogger(typeof(TService)) ?? NullLogManager.GetNullLogger(typeof(TService));
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
            return this.innerLogger.IsEnabled(level);
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
    }
}