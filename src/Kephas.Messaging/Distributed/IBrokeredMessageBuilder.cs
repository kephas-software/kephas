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
    using System.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    /// <summary>
    /// Interface for brokered message builder.
    /// </summary>
    [AppServiceContract]
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
    /// Extension methods for brokered message builders.
    /// </summary>
    public static class BrokeredMessageBuilderExtensions
    {
        /// <summary>
        /// Sets the content message.
        /// </summary>
        /// <param name="this">The message builder to act on.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public static IBrokeredMessageBuilder WithMessageContent(this IBrokeredMessageBuilder @this, object message)
        {
            Requires.NotNull(@this, nameof(@this));

            return @this.WithContent(message.ToMessageContent());
        }

        /// <summary>
        /// Sets the content message to an event.
        /// </summary>
        /// <param name="this">The message builder to act on.</param>
        /// <param name="event">The event.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public static IBrokeredMessageBuilder WithEventContent(this IBrokeredMessageBuilder @this, object @event)
        {
            Requires.NotNull(@this, nameof(@this));

            return @this.WithContent(@event.ToEventContent());
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="this">The brokered message builder to act on.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public static IBrokeredMessageBuilder WithRecipients(this IBrokeredMessageBuilder @this, params Uri[] recipients)
        {
            Requires.NotNull(@this, nameof(@this));

            return @this.WithRecipients(recipients.Select(r => new Endpoint(r)));
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="this">The brokered message builder to act on.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public static IBrokeredMessageBuilder WithRecipients(this IBrokeredMessageBuilder @this, IEnumerable<Uri> recipients)
        {
            Requires.NotNull(@this, nameof(@this));

            return @this.WithRecipients(recipients.Select(r => new Endpoint(r)));
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="this">The brokered message builder to act on.</param>
        /// <param name="recipientUris">The recipients as URIs.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public static IBrokeredMessageBuilder WithRecipients(this IBrokeredMessageBuilder @this, params string[] recipientUris)
        {
            Requires.NotNull(@this, nameof(@this));

            return @this.WithRecipients(recipientUris.Select(r => new Endpoint(new Uri(r))));
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="this">The brokered message builder to act on.</param>
        /// <param name="recipientUris">The recipients as URIs.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public static IBrokeredMessageBuilder WithRecipients(this IBrokeredMessageBuilder @this, IEnumerable<string> recipientUris)
        {
            Requires.NotNull(@this, nameof(@this));

            return @this.WithRecipients(recipientUris.Select(r => new Endpoint(new Uri(r))));
        }

        /// <summary>
        /// Converts the data to a message content.
        /// </summary>
        /// <param name="data">The data to act on.</param>
        /// <returns>
        /// Data as an IMessage.
        /// </returns>
        internal static IMessage ToMessageContent(this object data)
        {
            return data is IBrokeredMessage brokeredMessage
                ? brokeredMessage.Content
                : data is IMessage message
                    ? message
                    : new MessageAdapter { Message = data };
        }

        /// <summary>
        /// Converts the data to an event content.
        /// </summary>
        /// <param name="data">The data to act on.</param>
        /// <returns>
        /// Data as an IEvent.
        /// </returns>
        internal static IEvent ToEventContent(this object data)
        {
            if (data is IBrokeredMessage brokeredMessage)
            {
                data = brokeredMessage.Content;
            }

            return data is IEvent @event
                    ? @event
                    : new EventAdapter { Event = data };
        }
    }
}