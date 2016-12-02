// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default processing context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// The default processing context.
    /// </summary>
    public class MessageProcessingContext : ContextBase, IMessageProcessingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingContext"/> class.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided, <see cref="AmbientServices.Instance"/> will be considered.</param>
        public MessageProcessingContext(IMessage message, IMessageHandler handler, IAmbientServices ambientServices = null)
            : base(ambientServices)
        {
            Contract.Requires(message != null);
            Contract.Requires(handler != null);

            this.Message = message;
            this.Handler = handler;
        }

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