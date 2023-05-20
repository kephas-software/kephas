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
    public class MessagingContext : Context, IMessagingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IContext parentContext,
            IMessageBase message)
            : base(parentContext ?? throw new ArgumentNullException(nameof(parentContext)), merge: true)
        {
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IServiceProvider serviceProvider,
            IMessageBase message)
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
        public IMessageBase Message { get; }
    }
}