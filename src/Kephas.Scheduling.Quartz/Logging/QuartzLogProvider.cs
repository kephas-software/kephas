// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuartzLogProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quartz log provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.Logging
{
    using System;

    using global::Quartz.Logging;

    using Kephas.Logging;

    using LogLevel = Kephas.Logging.LogLevel;

    /// <summary>
    ///     A quartz log provider.
    /// </summary>
    public class QuartzLogProvider : ILogProvider
    {
        /// <summary>
        ///     Manager for log.
        /// </summary>
        private readonly ILogManager logManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuartzLogProvider" /> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public QuartzLogProvider(ILogManager logManager)
        {
            this.logManager = logManager;
        }

        /// <summary>
        ///     Gets a logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///     The logger.
        /// </returns>
        public Logger GetLogger(string name)
        {
            var logger = this.logManager.GetLogger(name);
            return (level, messageFunc, exception, formatParams) =>
            {
                if (messageFunc != null)
                {
                    logger.Log(this.GetLogLevel(level), exception, messageFunc(), formatParams);
                }

                return true;
            };
        }

        /// <summary>
        /// Opens a nested diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="message">The message to add to the diagnostics context.</param>
        /// <returns>
        /// A disposable that when disposed removes the message from the context.
        /// </returns>
        public IDisposable OpenNestedContext(string message)
        {
            return new NestedContext();
        }

        /// <summary>
        /// Opens a mapped diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <returns>
        /// A disposable that when disposed removes the map from the context.
        /// </returns>
        public IDisposable OpenMappedContext(string key, string value)
        {
            return new MappedContext();
        }

        /// <summary>
        /// Gets the Kephas log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        /// The Kephas log level.
        /// </returns>
        private LogLevel GetLogLevel(global::Quartz.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case global::Quartz.Logging.LogLevel.Fatal:
                    return LogLevel.Fatal;
                case global::Quartz.Logging.LogLevel.Error:
                    return LogLevel.Error;
                case global::Quartz.Logging.LogLevel.Warn:
                    return LogLevel.Warning;
                case global::Quartz.Logging.LogLevel.Info:
                    return LogLevel.Info;
                case global::Quartz.Logging.LogLevel.Trace:
                    return LogLevel.Trace;
                case global::Quartz.Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                default:
                    return LogLevel.Info;
            }
        }

        /// <summary>
        /// A nested context.
        /// </summary>
        private class NestedContext : IDisposable
        {
            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
            ///     resources.
            /// </summary>
            public void Dispose()
            {
            }
        }

        /// <summary>
        /// A mapped context.
        /// </summary>
        private class MappedContext : IDisposable
        {
            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
            ///     resources.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }
}