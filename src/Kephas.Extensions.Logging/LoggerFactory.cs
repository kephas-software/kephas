// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logger factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A logger factory.
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        private IList<ILoggerProvider> providers = new List<ILoggerProvider>();
        private ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        /// Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system.
        /// </summary>
        /// <param name="provider">The <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />.</param>
        public void AddProvider(ILoggerProvider provider)
        {
            this.providers.Add(provider);
            this.loggers.Clear();
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.
        /// </returns>
        public ILogger CreateLogger(string categoryName)
        {
            return this.loggers.GetOrAdd(
                categoryName,
                _ => new AggregatedLogger(this.providers.Select(p => p.CreateLogger(categoryName)).ToList()));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.providers.ForEach(p => p.Dispose());
            this.providers.Clear();
        }
    }
}
