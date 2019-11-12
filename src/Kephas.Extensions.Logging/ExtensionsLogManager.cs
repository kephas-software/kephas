// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsLogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the extensions log manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using Kephas.Logging;
    using Microsoft.Extensions.Logging;

    using ILogger = Kephas.Logging.ILogger;

    /// <summary>
    /// Manager for extensions logs.
    /// </summary>
    public class ExtensionsLogManager : ILogManager
    {
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsLogManager"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ExtensionsLogManager(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public ILogger GetLogger(string loggerName)
        {
            throw new System.NotImplementedException();
        }
    }
}
