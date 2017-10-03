// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message envelope class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A message envelope.
    /// </summary>
    public class BrokeredMessage : IBrokeredMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessage"/> class.
        /// </summary>
        public BrokeredMessage()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; }

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
        public IMessage Content { get; set; }

        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        /// <value>
        /// The recipients.
        /// </value>
        public IEnumerable<IEndpoint> Recipients { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public string Channel { get; set; }

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
        /// The default value <see cref="BrokeredMessageBuilder{TMessage}.DefaultTimeout"/> can be used.
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
        public object ReplyToMessageId { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder(this.GetType().Name);
            var contentType = this.Content?.GetType().Name;
            var recipients = this.Recipients != null ? string.Join(",", this.Recipients) : string.Empty;
            sb.Append($" (#{this.Id}) {{{contentType}/{this.Sender} > {recipients}}}");
            return sb.ToString();
        }
    }
}