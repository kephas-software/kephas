// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResponse.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IResponse interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Messaging.Messages
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// Interface for a simple response message.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        LogLevel Severity { get; set; }

        /// <summary>
        /// Gets or sets the informational message.
        /// </summary>
        /// <value>
        /// The informational message.
        /// </value>
        string? Message { get; set; }
    }
}
