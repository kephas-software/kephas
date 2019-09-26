// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Composition.AttributedModel;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Resources;
    using Kephas.Security.Authentication;
    using Kephas.Services;

    /// <summary>
    /// A brokered message builder.
    /// </summary>
    public class BrokeredMessageBuilder : IBrokeredMessageBuilder, IInitializable
    {
        /// <summary>
        /// The brokered message.
        /// </summary>
        private BrokeredMessage brokeredMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilder"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="authenticationService">The authentication service.</param>
        [CompositionConstructor]
        public BrokeredMessageBuilder(IAppManifest appManifest, IAuthenticationService authenticationService)
        {
            Requires.NotNull(appManifest, nameof(appManifest));
            Requires.NotNull(authenticationService, nameof(authenticationService));

            this.AppManifest = appManifest;
            this.AuthenticationService = authenticationService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilder"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="context">The context for initialization.</param>
        public BrokeredMessageBuilder(IAppManifest appManifest, IAuthenticationService authenticationService, IContext context)
            : this(appManifest, authenticationService)
        {
            Requires.NotNull(context, nameof(context));

            this.Initialize(context);
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
        IBrokeredMessage IBrokeredMessageBuilder.BrokeredMessage => this.brokeredMessage;

        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <value>
        /// The brokered message.
        /// </value>
        public BrokeredMessage BrokeredMessage => this.brokeredMessage;

        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        public IAppManifest AppManifest { get; }

        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        /// <value>
        /// The authentication service.
        /// </value>
        public IAuthenticationService AuthenticationService { get; }

        /// <summary>
        /// Gets the sending context.
        /// </summary>
        /// <value>
        /// The sending context.
        /// </value>
        public IContext Context { get; private set; }

        /// <summary>
        /// Sets the given brokered message for building. Must be set before calling <see cref="Initialize"/>.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public IBrokeredMessageBuilder Of(IBrokeredMessage brokeredMessage)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));

            this.brokeredMessage = (BrokeredMessage)brokeredMessage;
            this.brokeredMessage.BearerToken = this.GetBearerToken(this.Context);
            return this;
        }

        /// <summary>
        /// Sets the content message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.WithContent(IMessage message)
        {
            return this.WithContent(message);
        }

        /// <summary>
        /// Sets the content message. An event content makes the message one-way.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder WithContent(IMessage message)
        {
            if (this.brokeredMessage.ReplyToMessageId == null && message == null)
            {
                throw new ArgumentNullException(nameof(message), Strings.BrokeredMessageBuilder_ContentNullWhenNotReply_Exception);
            }

            if (message is IEvent)
            {
                this.brokeredMessage.IsOneWay = true;
            }

            this.brokeredMessage.Content = message;

            return this;
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.WithSender(string senderId)
        {
            return this.WithSender(senderId);
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder WithSender(string senderId)
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
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.WithSender(IEndpoint sender)
        {
            return this.WithSender(sender);
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder WithSender(IEndpoint sender)
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
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.WithSender(Uri sender)
        {
            return this.WithSender(sender);
        }

        /// <summary>
        /// Sets the sender of the brokered message.
        /// </summary>
        /// <param name="sender">The message sender.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder WithSender(Uri sender)
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
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.WithRecipients(params IEndpoint[] recipients)
        {
            return this.WithRecipients(recipients);
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder WithRecipients(params IEndpoint[] recipients)
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
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.WithRecipients(IEnumerable<IEndpoint> recipients)
        {
            return this.WithRecipients(recipients);
        }

        /// <summary>
        /// Sets the recipients to the brokered message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder WithRecipients(IEnumerable<IEndpoint> recipients)
        {
            Requires.NotNullOrEmpty(recipients, nameof(recipients));

            this.brokeredMessage.Recipients = recipients;

            return this;
        }

        /// <summary>
        /// Makes the communication one way.
        /// </summary>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.OneWay()
        {
            return this.OneWay();
        }

        /// <summary>
        /// Makes the communication one way.
        /// </summary>
        /// <returns>
        /// This <see cref="BrokeredMessageBuilder"/>.
        /// </returns>
        public virtual BrokeredMessageBuilder OneWay()
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
        IBrokeredMessageBuilder IBrokeredMessageBuilder.Timeout(TimeSpan timeout)
        {
            return this.Timeout(timeout);
        }

        /// <summary>
        /// Sets the timeout when waiting for an answer.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the timeout is negative.</exception>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        public virtual BrokeredMessageBuilder Timeout(TimeSpan timeout)
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
        IBrokeredMessageBuilder IBrokeredMessageBuilder.ReplyTo(string messageId, params IEndpoint[] recipients)
        {
            return this.ReplyTo(messageId, recipients);
        }

        /// <summary>
        /// Makes the message as a reply to another message.
        /// </summary>
        /// <param name="messageId">Identifier for the message.</param>
        /// <param name="recipients">A variable-length parameters list containing recipients.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        public virtual BrokeredMessageBuilder ReplyTo(string messageId, params IEndpoint[] recipients)
        {
            Requires.NotNull(messageId, nameof(messageId));

            this.brokeredMessage.ReplyToMessageId = messageId;
            this.brokeredMessage.IsOneWay = true;
            if (recipients != null)
            {
                this.brokeredMessage.Recipients = recipients;

            }

            return this;
        }

        /// <summary>
        /// Makes the message as a reply to another message.
        /// </summary>
        /// <param name="message">The message to reply to.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.ReplyTo(IBrokeredMessage message)
        {
            return this.ReplyTo(message);
        }

        /// <summary>
        /// Makes the message as a reply to another message.
        /// </summary>
        /// <param name="message">The message to reply to.</param>
        /// <returns>
        /// A BrokeredMessageBuilder.
        /// </returns>
        public virtual BrokeredMessageBuilder ReplyTo(IBrokeredMessage message)
        {
            Requires.NotNull(message, nameof(message));

            if (string.IsNullOrEmpty(this.brokeredMessage.BearerToken))
            {
                this.brokeredMessage.BearerToken = message.BearerToken;
            }

            return this.ReplyTo(message.Id, message.Sender);
        }

        /// <summary>
        /// Sets the channel to use.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        IBrokeredMessageBuilder IBrokeredMessageBuilder.UseChannel(string channel)
        {
            return this.UseChannel(channel);
        }

        /// <summary>
        /// Sets the channel to use.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        /// This <see cref="IBrokeredMessageBuilder"/>.
        /// </returns>
        public virtual IBrokeredMessageBuilder UseChannel(string channel)
        {
            Requires.NotNull(channel, nameof(channel));

            this.brokeredMessage.Channel = channel;

            return this;
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        public void Initialize(IContext context = null)
        {
            this.Context = context;

            // ReSharper disable once VirtualMemberCallInConstructor
            this.Of(this.CreateBrokeredMessage());
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
                Timeout = DefaultTimeout,
                Sender = this.CreateEndpoint(null),
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
            return new Endpoint(this.AppManifest.AppId, this.AppManifest.AppInstanceId, senderId, scheme: this.BrokeredMessage?.Channel);
        }

        /// <summary>
        /// Gets the bearer token.
        /// </summary>
        /// <param name="context">The sending context (optional).</param>
        /// <returns>
        /// The bearer token.
        /// </returns>
        protected virtual string GetBearerToken(IContext context)
        {
            return this.AuthenticationService.GetToken(context?.Identity, context)?.ToString();
        }
    }
}