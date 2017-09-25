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
    }
}