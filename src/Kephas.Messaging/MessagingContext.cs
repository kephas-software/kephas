// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Services;
    using Kephas.Services;

    /// <summary>
    /// The messaging context.
    /// </summary>
    public class MessagingContext<TMessage, TResult> : Context, IMessagingContext<TMessage, TResult>
        where TMessage : IMessage<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext{TMessage, TResult}"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IContext parentContext,
            IMessage? message)
            : base(parentContext, merge: true)
        {
            parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));

            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext{TMessage, TResult}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IServiceProvider serviceProvider,
            IMessage? message)
            : base(serviceProvider)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IMessage? Message { get; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public object? Result { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public object? Response { get; set; }
    }
}