// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingBackMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The "pong" response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using System;

    /// <summary>
    /// The "ping back" response.
    /// </summary>
    public class PingBackMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the server time.
        /// </summary>
        /// <value>
        /// The server time.
        /// </value>
        public DateTimeOffset ServerTime { get; set; }
    }
}