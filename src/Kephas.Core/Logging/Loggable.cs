// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Loggable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the loggable class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    /// <summary>
    /// A loggable mixin class.
    /// </summary>
    public class Loggable : ILoggable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger
        {
            get => this.logger ?? (this.logger = this.GetLogger());
            protected internal set => this.logger = value;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns>
        /// The logger.
        /// </returns>
        protected virtual ILogger GetLogger()
        {
            return this.GetLogger(null);
        }
    }
}