// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingContextExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message processing context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    /// <summary>
    /// A message processing context extensions.
    /// </summary>
    public static class MessageProcessingContextExtensions
    {
        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <param name="processingContext">The processing context to act on.</param>
        /// <returns>
        /// The brokered message.
        /// </returns>
        public static IBrokeredMessage GetBrokeredMessage(this IMessageProcessingContext processingContext)
        {
            return processingContext["BrokeredMessage"] as IBrokeredMessage;
        }

        /// <summary>
        /// Sets the brokered message.
        /// </summary>
        /// <param name="processingContext">The processing context to act on.</param>
        /// <param name="value">The value.</param>
        public static void SetBrokeredMessage(this IMessageProcessingContext processingContext, IBrokeredMessage value)
        {
            processingContext["BrokeredMessage"] = value;
        }
    }
}