// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoggable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILoggable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Logging
{
    /// <summary>
    /// Interface for loggable objects.
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        ILogger? Logger { get; }
    }
}