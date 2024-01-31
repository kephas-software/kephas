// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Response.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the information response message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Messaging.Messages
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// A simple response message.
    /// </summary>
    public class Response : IResponse
    {
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public LogLevel Severity { get; set; } = LogLevel.Information;

        /// <summary>
        /// Gets or sets the informational message.
        /// </summary>
        /// <value>
        /// The informational message.
        /// </value>
        public string? Message { get; set; }
    }
}
