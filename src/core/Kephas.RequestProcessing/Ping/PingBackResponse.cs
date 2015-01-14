// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingBackResponse.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The "pong" response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Ping
{
    using System;

    /// <summary>
    /// The "ping back" response.
    /// </summary>
    public class PingBackResponse : IResponse
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