// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default processing context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// The default processing context.
    /// </summary>
    public class MessageProcessingContext : Context, IMessageProcessingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingContext"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="message">The Message.</param>
        /// <param name="handler">The handler.</param>
        public MessageProcessingContext(IMessageProcessor messageProcessor, IMessage message, IMessageHandler handler)
            : base(messageProcessor.AmbientServices)
        {
            Contract.Requires(messageProcessor != null);
            Requires.NotNull(message, nameof(message));
            Contract.Requires(handler != null);

            this.MessageProcessor = messageProcessor;
            this.Message = message;
            this.Handler = handler;
        }

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        public IMessageProcessor MessageProcessor { get; }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public IMessageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public IMessage Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }
    }
}