// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPluginDataException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;

    /// <summary>
    /// Exception for signalling invalid plugin data errors.
    /// </summary>
    public class InvalidPluginDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPluginDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidPluginDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPluginDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidPluginDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}