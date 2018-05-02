// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logger factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Logging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A logger factory.
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// The ambient services.
        /// </summary>
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFactory"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public LoggerFactory(IAmbientServices ambientServices)
        {
            this.ambientServices = ambientServices;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            var logger = this.ambientServices.GetLogger(categoryName);
            return new AspNetLogger(logger);
        }

        /// <summary>
        /// Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system.
        /// </summary>
        /// <param name="provider">The <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />.</param>
        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}