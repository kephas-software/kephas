// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingBackMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The "pong" response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Ping
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