// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message envelope class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Services;

    /// <summary>
    /// A message envelope.
    /// </summary>
    public class BrokeredMessage : IBrokeredMessage
    {
        private string id;
        private IMessage content;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessage"/> class.
        /// </summary>
        public BrokeredMessage()
        {
            this.id = Guid.NewGuid().ToString("N");
            this.Timeout = DefaultTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BrokeredMessage(object message)
            : this()
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            this.Content = message.ToMessage()!;
        }

        /// <summary>
        /// Gets or sets the default timeout when waiting for a response.
        /// The default value is 30 seconds, but it can be changed to accomodate application needs.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get => this.id;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new System.ArgumentException("Value must not be null or empty.", nameof(value));
                this.id = value;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.id;

        /// <summary>
        /// Gets or sets the sender of the message.
        /// </summary>
        /// <value>
        /// The message sender.
        /// </value>
        public IEndpoint Sender { get; set; }

        /// <summary>
        /// Gets or sets the message to send.
        /// </summary>
        /// <value>
        /// The message to send.
        /// </value>
        public IMessage Content
        {
            get => this.content;
            set
            {
                if (this.ReplyToMessageId == null && value == null)
                {
                    throw new ArgumentNullException(
                        nameof(value),
                        Strings.BrokeredMessage_ContentNullWhenNotReply_Exception
                            .FormatWith(this, nameof(DispatchingContextExtensions.ReplyTo)));
                }

                if (value is IMessageEnvelope envelope && envelope.GetContent() is Delegate)
                {
                    throw new ArgumentException(nameof(value), Strings.BrokeredMessage_ContentCannotBeDelegate_Exception.FormatWith(value));
                }

                this.content = value;
                if (value is IEvent)
                {
                    this.IsOneWay = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        /// <value>
        /// The recipients.
        /// </value>
        public IEnumerable<IEndpoint>? Recipients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this message is one way.
        /// </summary>
        /// <value>
        /// True if this message is one way, false if not.
        /// </value>
        public bool IsOneWay { get; set; }

        /// <summary>
        /// Gets or sets the timeout when waiting for responses.
        /// </summary>
        /// <remarks>
        /// A value of <c>null</c> means indefinitely waiting, but
        /// it is strongly discouraged to wait indefinitely for a response.
        /// The value <see cref="DefaultTimeout"/> is used by default.
        /// </remarks>
        /// <value>
        /// The response timeout.
        /// </value>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the message to reply to.
        /// </summary>
        /// <value>
        /// The identifier of the reply to message.
        /// </value>
        public string ReplyToMessageId { get; set; }

        /// <summary>
        /// Gets or sets the bearer token.
        /// </summary>
        /// <value>
        /// The bearer token.
        /// </value>
        public string BearerToken { get; set; }

        /// <summary>
        /// Gets or sets the custom properties of the brokered message.
        /// </summary>
        /// <value>
        /// The custom properties.
        /// </value>
        public IDictionary<string, object?>? Properties { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public Priority Priority { get; set; }

        /// <summary>
        /// Gets or sets the trace.
        /// </summary>
        /// <value>
        /// The trace.
        /// </value>
        public string Trace { get; set; }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// The values will be set or retrieved using the <see cref="Properties"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public object? this[string key]
        {
            get => (this.Properties ??= new Dictionary<string, object?>())[key];
            set => (this.Properties ??= new Dictionary<string, object?>())[key] = value;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var contentType = this.Content?.GetType().Name;
            var recipients = this.Recipients != null ? string.Join(",", this.Recipients) : string.Empty;
            var oneway = this.IsOneWay ? ", one way" : string.Empty;
            var reply = string.IsNullOrEmpty(this.ReplyToMessageId) ? string.Empty : $", reply to #{this.ReplyToMessageId}";
            return $"{this.GetType().Name} (#{this.Id}) {{{contentType}/{this.Sender} > {recipients}}}{oneway}{reply}, {this.Priority} priority";
        }

        /// <summary>
        /// Makes a deep copy of this object, optionally replacing the existing recipients with the
        /// provided ones.
        /// </summary>
        /// <param name="recipients">Optional. The recipients.</param>
        /// <returns>
        /// A copy of this object.
        /// </returns>
        public IBrokeredMessage Clone(IEnumerable<IEndpoint>? recipients = null)
        {
            return new BrokeredMessage
            {
                Id = this.Id,
                Sender = this.Sender,
                Recipients = recipients ?? this.Recipients?.ToArray(),
                BearerToken = this.BearerToken,
                content = this.Content,             // write directly into the content field, to avoid validations
                IsOneWay = this.IsOneWay,
                Properties = this.Properties == null ? null : new Dictionary<string, object?>(this.Properties),
                Priority = this.Priority,
                ReplyToMessageId = this.ReplyToMessageId,
                Timeout = this.Timeout,
                Trace = this.Trace,
            };
        }
    }
}