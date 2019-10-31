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
        private const string BrokeredMessageKey = "BrokeredMessage";

        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <param name="processingContext">The processing context to act on.</param>
        /// <returns>
        /// The brokered message.
        /// </returns>
        public static IBrokeredMessage GetBrokeredMessage(this IMessagingContext processingContext)
        {
            return processingContext[BrokeredMessageKey] as IBrokeredMessage;
        }

        /// <summary>
        /// Sets the brokered message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="processingContext">The processing context to act on.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This <paramref name="processingContext"/>.
        /// </returns>
        public static TContext SetBrokeredMessage<TContext>(this TContext processingContext, IBrokeredMessage value)
            where TContext : class, IMessagingContext
        {
            processingContext[BrokeredMessageKey] = value;
            return processingContext;
        }
    }
}