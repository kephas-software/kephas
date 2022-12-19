// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoggable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILoggable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    /// <summary>
    /// Interface for loggable objects.
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Gets the log level of the loggable instance.
        /// </summary>
        /// <returns>The log level.</returns>
        LogLevel GetLogLevel() => LogLevel.Info;
    }
}