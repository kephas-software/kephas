// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default processing context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.Data;
using Kephas.Services;

namespace Kephas.Messaging
{
    /// <summary>
    /// The messaging context.
    /// </summary>
    public class MessagingContext : Context, IMessagingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IContext parentContext,
            IMessageMatchService messageMatchService,
            IMessageBase message)
            : base(parentContext ?? throw new ArgumentNullException(nameof(parentContext)), merge: true)
        {
            InitializeContext(message, messageMatchService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="messageMatchService">The message match service.</param>
        /// <param name="message">Optional. The message.</param>
        public MessagingContext(
            IServiceProvider serviceProvider,
            IMessageMatchService messageMatchService,
            IMessageBase message)
            : base(serviceProvider)
        {
            InitializeContext(message, messageMatchService);
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IMessageBase Message { get; private set; }

        /// <summary>
        /// Gets the envelope type.
        /// </summary>
        public Type EnvelopeType { get; private set; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public Type MessageType { get; private set; }

        /// <summary>
        /// Gets the correlation ID of this invocation.
        /// </summary>
        public string CorrelationId { get; private set; }

#if NET6_0_OR_GREATER        
        [MemberNotNull(nameof(EnvelopeType))]
        [MemberNotNull(nameof(MessageType))]
        [MemberNotNull(nameof(Message))]
        [MemberNotNull(nameof(CorrelationId))]
#endif
        private void InitializeContext(IMessageBase message, IMessageMatchService matchService)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            EnvelopeType = matchService.GetEnvelopeType(message);
            MessageType = matchService.GetMessageType(message);
            CorrelationId = (message as ICorrelated)?.CorrelationId ?? Guid.NewGuid().ToString("N");
        }
    }
}