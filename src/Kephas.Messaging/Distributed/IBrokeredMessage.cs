// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBrokeredMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageEnvelope interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;

    using Kephas.Data;

    /// <summary>
    /// Contract interface for brokered messages.
    /// </summary>
    public interface IBrokeredMessage : IIdentifiable, IMessage
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        new string Id { get; }

        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        /// <value>
        /// The message sender.
        /// </value>
        IEndpoint Sender { get; }

        /// <summary>
        /// Gets the message to send.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        IMessage Content { get; }

        /// <summary>
        /// Gets the recipients.
        /// </summary>
        /// <value>
        /// The recipients.
        /// </value>
        IEnumerable<IEndpoint> Recipients { get; }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        string Channel { get; }

        /// <summary>
        /// Gets a value indicating whether this message is one way.
        /// </summary>
        /// <value>
        /// True if this message is one way, false if not.
        /// </value>
        bool IsOneWay { get; }

        /// <summary>
        /// Gets the timeout when waiting for responses.
        /// </summary>
        /// <remarks>
        /// A value of <c>null</c> means indefinitely waiting, but
        /// it is strongly discouraged to wait indefinitely for a response.
        /// The default value <see cref="BrokeredMessageBuilder{T}.DefaultTimeout"/> can be used.
        /// </remarks>
        /// <value>
        /// The response timeout.
        /// </value>
        TimeSpan? Timeout { get; }

        /// <summary>
        /// Gets the identifier of the message to reply to.
        /// </summary>
        /// <value>
        /// The identifier of the reply to message.
        /// </value>
        string ReplyToMessageId { get; }
    }
}