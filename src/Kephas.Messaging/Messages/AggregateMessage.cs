// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aggregate message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// An aggregate message.
    /// </summary>
    public class AggregateMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public IEnumerable<IMessage> Messages { get; set; }
    }
}
