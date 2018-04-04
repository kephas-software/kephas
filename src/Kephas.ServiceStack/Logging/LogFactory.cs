// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the log factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Logging
{
    using System;

    using global::ServiceStack.Logging;

    using Kephas.Logging;

    /// <summary>
    /// A log factory adapter for Service Stack.
    /// </summary>
    public class LogFactory : ILogFactory
    {
        /// <summary>
        /// The log manager.
        /// </summary>
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFactory"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public LogFactory(ILogManager logManager)
        {
            this.logManager = logManager;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public ILog GetLogger(Type type)
        {
            var logger = this.logManager.GetLogger(type);
            return new Log(logger);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public ILog GetLogger(string typeName)
        {
            var logger = this.logManager.GetLogger(typeName);
            return new Log(logger);
        }
    }
}