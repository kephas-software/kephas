// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageMatchService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageMatchService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for message match service.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IMessageMatchService
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The message type.
        /// </returns>
        Type GetMessageType(object message);

        /// <summary>
        /// Gets the message ID.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The message ID.
        /// </returns>
        object GetMessageId(object message);

        /// <summary>
        /// Checks whether the message type and message ID matches the provided criteria.
        /// </summary>
        /// <param name="messageMatch">Provides the matching criteria.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageId">Identifier for the message.</param>
        /// <returns>
        /// True if the message type and ID matches the criteria, false if not.
        /// </returns>
        bool IsMatch(IMessageMatch messageMatch, Type messageType, object messageId);
    }

    /// <summary>
    /// A message match service extensions.
    /// </summary>
    public static class MessageMatchServiceExtensions
    {
        /// <summary>
        /// Checks whether the message matches the provided criteria.
        /// </summary>
        /// <param name="messageMatchService">The message match service to act on.</param>
        /// <param name="messageMatch">Provides the matching criteria.</param>
        /// <param name="message">The message to check.</param>
        /// <returns>
        /// True if the message matches the criteria, false if not.
        /// </returns>
        public static bool IsMatch(
            this IMessageMatchService messageMatchService,
            IMessageMatch messageMatch,
            IMessage message)
        {
            Requires.NotNull(messageMatchService, nameof(messageMatchService));

            return messageMatchService.IsMatch(
                messageMatch,
                messageMatchService.GetMessageType(message),
                messageMatchService.GetMessageId(message));
        }
    }
}