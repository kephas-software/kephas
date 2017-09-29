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

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A brokered message builder.
    /// </summary>
    public class BrokeredMessageBuilder
    {
        /// <summary>
        /// The application manifest.
        /// </summary>
        private readonly IAppManifest appManifest;

        /// <summary>
        /// The brokered message.
        /// </summary>
        private BrokeredMessage brokeredMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilder"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        public BrokeredMessageBuilder(IAppManifest appManifest)
        {
            Requires.NotNull(appManifest, nameof(appManifest));

            this.appManifest = appManifest;

            // ReSharper disable once VirtualMemberCallInConstructor
            this.brokeredMessage = this.CreateBrokeredMessage();
            // ReSharper disable once VirtualMemberCallInConstructor
            this.brokeredMessage.Sender = this.CreateEndpoint(null);
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
        /// Sets the message to broker.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public BrokeredMessageBuilder WithContent(IMessage message)
        {
            Requires.NotNull(message, nameof(message));

            this.brokeredMessage.Content = message;

            return this;
        }

        /// <summary>
        /// Sets the message to broker.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public BrokeredMessageBuilder WithSender(string senderId)
        {
            Requires.NotNullOrEmpty(senderId, nameof(senderId));

            this.brokeredMessage.Sender = this.CreateEndpoint(senderId);

            return this;
        }

        /// <summary>
        /// Sets the message to broker.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public BrokeredMessageBuilder WithSender(IEndpoint sender)
        {
            Requires.NotNull(sender, nameof(sender));

            this.brokeredMessage.Sender = sender;

            return this;
        }

        /// <summary>
        /// Makes the communication one way.
        /// </summary>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public BrokeredMessageBuilder OneWay()
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
        public BrokeredMessageBuilder Timeout(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                // TODO localization
                throw new ArgumentException("Cannot specify a negative time span.", nameof(timeout));
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
        public BrokeredMessageBuilder ReplyTo(object messageId, params IEndpoint[] recipients)
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
        protected virtual BrokeredMessage CreateBrokeredMessage()
        {
            return new BrokeredMessage
                       {
                           Timeout = DefaultTimeout
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
            return new Endpoint
                {
                    EndpointId = senderId,
                    AppId = this.appManifest.AppId,
                    AppInstanceId = this.appManifest.AppInstanceId,
                };
        }
    }
}