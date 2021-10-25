// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypedLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// NLog logger for the <typeparamref name="TTargetContract"/>.
    /// </summary>
    /// <typeparam name="TTargetContract">The type of the target service contract.</typeparam>
    [OverridePriority(Priority.Low)]
    public class TypedLogger<TTargetContract> : ILogger<TTargetContract>
    {
        /// <summary>
        /// The inner logger.
        /// </summary>
        private readonly ILogger innerLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedLogger{TTargetContract}"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public TypedLogger(ILogManager logManager)
        {
            logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));

            this.innerLogger = logManager.GetLogger(typeof(TTargetContract)) ?? NullLogManager.GetNullLogger(typeof(TTargetContract));
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
        /// <param name="level">The logging level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        public bool Log(LogLevel level, Exception? exception, string? messageFormat, params object?[] args)
        {
            return this.innerLogger.Log(level, exception, messageFormat, args);
        }
    }
}