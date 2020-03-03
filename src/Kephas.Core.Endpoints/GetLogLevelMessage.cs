// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLogLevelMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get log level message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A get log level message.
    /// </summary>
    [TypeDisplay(Description = "Gets the application minimum log level.")]
    public class GetLogLevelMessage : IMessage
    {
    }

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