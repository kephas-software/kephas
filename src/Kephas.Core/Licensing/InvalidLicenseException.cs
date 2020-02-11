// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidLicenseException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the licensing manager base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System;

    /// <summary>
    /// Exception for signalling invalid license errors.
    /// </summary>
    public class InvalidLicenseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLicenseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidLicenseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLicenseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidLicenseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}