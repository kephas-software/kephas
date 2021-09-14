// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLogLevelResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.Logging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A get log level response message.
    /// </summary>
    public class GetLogLevelResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        /// <value>
        /// The minimum log level.
        /// </value>
        public LogLevel MinimumLevel { get; set; }
    }
}