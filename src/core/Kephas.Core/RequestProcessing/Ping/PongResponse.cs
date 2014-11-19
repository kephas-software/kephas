// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PongResponse.cs" company="Quartz Software SRL">
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
    /// The "pong" response.
    /// </summary>
    public class PongResponse : IResponse
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