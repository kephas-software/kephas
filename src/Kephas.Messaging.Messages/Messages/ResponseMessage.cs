// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the information response message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// A simple response message.
    /// </summary>
    public class ResponseMessage : IResponseMessage
    {
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; set; } = SeverityLevel.Info;

        /// <summary>
        /// Gets or sets the informational message.
        /// </summary>
        /// <value>
        /// The informational message.
        /// </value>
        public string? Message { get; set; }
    }
}