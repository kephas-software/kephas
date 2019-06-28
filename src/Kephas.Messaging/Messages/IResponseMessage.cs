// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IResponseMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// Interface for a simple response message.
    /// </summary>
    public interface IResponseMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        SeverityLevel Severity { get; set; }

        /// <summary>
        /// Gets or sets the informational message.
        /// </summary>
        /// <value>
        /// The informational message.
        /// </value>
        string Message { get; set; }
    }
}