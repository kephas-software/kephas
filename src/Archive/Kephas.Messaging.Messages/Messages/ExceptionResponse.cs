// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionResponse.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// An exception message.
    /// </summary>
    public class ExceptionResponse
    {
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public ExceptionData Exception { get; set; }
    }
}