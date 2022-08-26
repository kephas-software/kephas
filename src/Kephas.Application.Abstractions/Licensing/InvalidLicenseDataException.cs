// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidLicenseDataException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the license class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System;

    /// <summary>
    /// Exception for signalling invalid license data errors.
    /// </summary>
    public class InvalidLicenseDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLicenseDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidLicenseDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLicenseDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidLicenseDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}