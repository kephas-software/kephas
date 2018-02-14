// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the brokered message builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Resources;

    /// <summary>
    /// A brokered message builder.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public class BrokeredMessageBuilder<TMessage>
        where TMessage : BrokeredMessage, new()
    {
        /// <summary>
        /// The application manifest.
        /// </summary>
        private readonly IAppManifest appManifest;

        /// <summary>
        /// The brokered message.
        /// </summary>
        private readonly TMessage brokeredMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilder{TMessage}"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        public BrokeredMessageBuilder(IAppManifest appManifest)
        {
            Requires.NotNull(appManifest, nameof(appManifest));

            this.appManifest = appManifest;

            // ReSharper disable once VirtualMemberCallInConstructor
            this.brokeredMessage = this.CreateBrokeredMessage();
        }

        /// <summary>
        /// Gets the default timeout.
        /// </summary>
        public static TimeSpan DefaultTimeout => TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <value>
        /// The brokered message.
        /// </value>
        public IBrokeredMessage BrokeredMessage => this.brokeredMessage;

        /// <summary>
        /// Sets the content message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> WithContent(IMessage message)
        {
            if (this.brokeredMessage.ReplyToMessageId == null && message == null)
            {
                throw new ArgumentNullException(nameof(message), Strings.BrokeredMessageBuilder_ContentNullWhenNotReply_Exception);
            }

            this.brokeredMessage.Content = message;

            return this;
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> WithSender(string senderId)
        {
            Requires.NotNullOrEmpty(senderId, nameof(senderId));

            this.brokeredMessage.Sender = this.CreateEndpoint(senderId);

            return this;
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> WithSender(IEndpoint sender)
        {
            Requires.NotNull(sender, nameof(sender));

            this.brokeredMessage.Sender = sender;

            return this;
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> WithSender(Uri sender)
        {
            Requires.NotNull(sender, nameof(sender));

            this.brokeredMessage.Sender = new Endpoint(sender);

            return this;
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> WithRecipients(params IEndpoint[] recipients)
        {
            Requires.NotNullOrEmpty(recipients, nameof(recipients));

            this.brokeredMessage.Recipients = recipients;

            return this;
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> WithRecipients(IEnumerable<IEndpoint> recipients)
        {
            Requires.NotNullOrEmpty(recipients, nameof(recipients));

            this.brokeredMessage.Recipients = recipients;

            return this;
        }

        /// <summary>
        /// Makes the communication one way.
        /// </summary>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder{TMessage}"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> OneWay()
        {
            this.brokeredMessage.IsOneWay = true;

            return this;
        }

        /// <summary>
        /// Sets the timeout when waiting for an answer.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the timeout is negative.</exception>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> Timeout(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentException(Strings.BrokeredMessageBuilder_NonNegativeTimeout_Exception, nameof(timeout));
            }

            this.brokeredMessage.Timeout = timeout;

            return this;
        }

        /// <summary>
        /// Makes the message as a reply to another message.
        /// </summary>
        /// <param name="messageId">Identifier for the message.</param>
        /// <param name="recipients">A variable-length parameters list containing recipients.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        public virtual BrokeredMessageBuilder<TMessage> ReplyTo(string messageId, params IEndpoint[] recipients)
        {
            Requires.NotNull(messageId, nameof(messageId));

            this.brokeredMessage.ReplyToMessageId = messageId;
            this.brokeredMessage.Recipients = recipients;

            return this;
        }

        /// <summary>
        /// Creates the brokered message.
        /// </summary>
        /// <returns>
        /// The new brokered message.
        /// </returns>
        protected virtual TMessage CreateBrokeredMessage()
        {
            return new TMessage
            {
                Timeout = DefaultTimeout,
                Sender = this.CreateEndpoint(null)
            };
        }

        /// <summary>
        /// Creates an endpoint.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <returns>
        /// The new endpoint.
        /// </returns>
        protected virtual IEndpoint CreateEndpoint(string senderId)
        {
            return new Endpoint(this.appManifest.AppId, this.appManifest.AppInstanceId, senderId);
        }
    }
}