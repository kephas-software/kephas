// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageHandlerProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message handler provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.HandlerProviders
{
    using System;

    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.HandlerProviders;
    using Kephas.Services;

    /// <summary>
    /// A brokered message handler provider.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class BrokeredMessageHandlerProvider : SingleMessageHandlerProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageHandlerProvider"/> class.
        /// </summary>
        /// <param name="messageMatchService">The message match service.</param>
        public BrokeredMessageHandlerProvider(IMessageMatchService messageMatchService)
            : base(messageMatchService)
        {
        }

        /// <summary>
        /// Indicates whether the selector can handle the indicated message type.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// True if the selector can handle the message type, false if not.
        /// </returns>
        public override bool CanHandle(IMessagingContext context)
        {
            return typeof(IBrokeredMessage).IsAssignableFrom(messageType);
        }
    }
}