// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// The logging helper class.
    /// </summary>
    public static class LoggingHelper
    {
        private static ILogManager logManager = new NullLogManager();

        /// <summary>
        /// Gets or sets the default manager for log.
        /// </summary>
        /// <value>
        /// The default log manager.
        /// </value>
        public static ILogManager DefaultLogManager
        {
            get => logManager;
            set
            {
                Requires.NotNull(value, nameof(value));
                logManager = value;
            }
        }
    }
}
