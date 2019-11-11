// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logger provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using Kephas.Logging;
    using Microsoft.Extensions.Logging;

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    /// <summary>
    /// A logger provider.
    /// </summary>
    public class LoggerProvider : ILoggerProvider
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerProvider"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public LoggerProvider(ILogManager logManager)
        {
            this.logManager = logManager;
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>
        /// The new logger.
        /// </returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(this.logManager.GetLogger(categoryName));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
