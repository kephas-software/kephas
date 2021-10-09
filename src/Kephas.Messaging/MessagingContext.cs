﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default processing context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// The messaging context.
    /// </summary>
    public class MessagingContext : Context, IMessagingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IContext parentContext,
            IMessageProcessor messageProcessor,
            IMessage? message = null)
            : base(parentContext, merge: true)
        {
            parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));
            messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));

            this.MessageProcessor = messageProcessor;
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="message">Optional. The Message.</param>
        public MessagingContext(
            IInjector injector,
            IMessageProcessor messageProcessor,
            IMessage? message = null)
            : base(injector)
        {
            messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));

            this.MessageProcessor = messageProcessor;
            this.Message = message;
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
        public IMessage? Message { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public IMessage? Response { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception? Exception { get; set; }
    }
}