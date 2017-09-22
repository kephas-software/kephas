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
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A brokered message builder.
    /// </summary>
    public class BrokeredMessageBuilder
    {
        /// <summary>
        /// The brokered message.
        /// </summary>
        private BrokeredMessage brokeredMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageBuilder"/> class.
        /// </summary>
        public BrokeredMessageBuilder()
        {
            this.brokeredMessage = this.CreateBrokeredMessage();
        }

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
        public BrokeredMessageBuilder WithMessage(IMessage message)
        {
            Requires.NotNull(message, nameof(message));

            this.brokeredMessage.Message = message;

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
        /// Creates the brokered message.
        /// </summary>
        /// <returns>
        /// The new brokered message.
        /// </returns>
        protected virtual BrokeredMessage CreateBrokeredMessage()
        {
            return new BrokeredMessage();
        }
    }
}