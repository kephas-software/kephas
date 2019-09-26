// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message processing context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    /// <summary>
    /// Extension methods for <see cref="IMessagingContext"/>.
    /// </summary>
    public static class MessagingContextExtensions
    {
        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <param name="processingContext">The processing context to act on.</param>
        /// <returns>
        /// The brokered message.
        /// </returns>
        public static IBrokeredMessage GetBrokeredMessage(this IMessagingContext processingContext)
        {
            return processingContext["BrokeredMessage"] as IBrokeredMessage;
        }

        /// <summary>
        /// Sets the brokered message.
        /// </summary>
        /// <param name="processingContext">The processing context to act on.</param>
        /// <param name="value">The value.</param>
        public static void SetBrokeredMessage(this IMessagingContext processingContext, IBrokeredMessage value)
        {
            processingContext["BrokeredMessage"] = value;
        }
    }
}