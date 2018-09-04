// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageMatch.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageMatch interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Messaging.Composition;

    /// <summary>
    /// Contract for message matching criteria.
    /// </summary>
    public interface IMessageMatch : IExpando
    {
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        Type MessageType { get; }

        /// <summary>
        /// Gets the identifier of the message.
        /// </summary>
        /// <value>
        /// The identifier of the message.
        /// </value>
        object MessageId { get; }

        /// <summary>
        /// Gets the message type matching.
        /// </summary>
        /// <value>
        /// The message type matching.
        /// </value>
        MessageTypeMatching MessageTypeMatching { get; }

        /// <summary>
        /// Gets the message identifier matching.
        /// </summary>
        /// <value>
        /// The message identifier matching.
        /// </value>
        MessageIdMatching MessageIdMatching { get; }
    }
}