// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBrokeredMessageBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBrokeredMessageBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for brokered message builder.
    /// </summary>
    public interface IBrokeredMessageBuilder
    {
        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <value>
        /// The brokered message.
        /// </value>
        IBrokeredMessage BrokeredMessage { get; }

        /// <summary>
        /// Sets the given brokered message for building.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder Of(IBrokeredMessage brokeredMessage);

        /// <summary>
        /// Sets the content message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder WithContent(IMessage message);

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder WithSender(string senderId);

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder WithSender(IEndpoint sender);

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder WithSender(Uri sender);

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder WithRecipients(params IEndpoint[] recipients);

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder WithRecipients(IEnumerable<IEndpoint> recipients);

        /// <summary>
        /// Makes the communication one way.
        /// </summary>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder OneWay();

        /// <summary>
        /// Sets the timeout when waiting for an answer.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the timeout is negative.</exception>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        IBrokeredMessageBuilder Timeout(TimeSpan timeout);

        /// <summary>
        /// Makes the message as a reply to another message.
        /// </summary>
        /// <param name="messageId">Identifier for the message.</param>
        /// <param name="recipients">A variable-length parameters list containing recipients.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        IBrokeredMessageBuilder ReplyTo(string messageId, params IEndpoint[] recipients);

        /// <summary>
        /// Makes the message as a reply to another message.
        /// </summary>
        /// <param name="message">The message to reply to.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        IBrokeredMessageBuilder ReplyTo(IBrokeredMessage message);
    }

    /// <summary>
    /// Interface for brokered message builder.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    [AppServiceContract(ContractType = typeof(IBrokeredMessageBuilder), AllowMultiple = true)]
    public interface IBrokeredMessageBuilder<out TMessage> : IBrokeredMessageBuilder
        where TMessage : IBrokeredMessage
    {
        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <value>
        /// The brokered message.
        /// </value>
        new TMessage BrokeredMessage { get; }
    }
}