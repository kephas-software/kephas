// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;

    /// <summary>
    /// Occurs when there are errors in the configuration.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}