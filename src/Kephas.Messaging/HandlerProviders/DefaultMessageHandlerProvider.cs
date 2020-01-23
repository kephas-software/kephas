// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message handler provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.HandlerProviders
{
    using System;

    using Kephas.Messaging;
    using Kephas.Services;

    /// <summary>
    /// A default message handler provider.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class DefaultMessageHandlerProvider : SingleMessageHandlerProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageHandlerProvider"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        public DefaultMessageHandlerProvider(IMessageMatchService messageMatchService)
            : base(messageMatchService)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="envelopeType">The type of the envelope. This is typically the adapter type, if the message does not implement <see cref="IMessage"/>.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(Type envelopeType, Type messageType, object messageId)
        {
            return true;
        }
    }
}