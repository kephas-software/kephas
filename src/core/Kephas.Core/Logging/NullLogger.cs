// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Default implementation of a logger doing nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Default implementation of a logger doing nothing.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    [OverridePriority(Priority.Lowest)]
    public class NullLogger<TService> : ILogger<TService>
    {
        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Log(LogLevel level, object message, Exception exception = null)
        {
        }

        /// <summary>
        /// Logs the information at the provided level.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public void Log(LogLevel level, string messageFormat, params object[] args)
        {
        }
    }
}