// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageMatchService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageMatchService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging;

using System;

using Kephas.Services;

/// <summary>
/// Interface for message match service.
/// </summary>
[SingletonAppServiceContract]
public interface IMessageMatchService
{
    /// <summary>
    /// Gets the envelope type.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>
    /// The envelope type.
    /// </returns>
    Type GetEnvelopeType(object message) => message.GetType();

    /// <summary>
    /// Gets the message type.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>
    /// The message type.
    /// </returns>
    Type GetMessageType(object message);

    /// <summary>
    /// Checks whether the message type and message ID matches the provided criteria.
    /// </summary>
    /// <param name="messageMatch">Provides the matching criteria.</param>
    /// <param name="envelopeType">Type of the envelope.</param>
    /// <param name="messageType">Type of the message.</param>
    /// <returns>
    /// True if the message type and ID matches the criteria, false if not.
    /// </returns>
    bool IsMatch(IMessageMatch messageMatch, Type envelopeType, Type messageType);
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
        IMessageBase message)
    {
        _ = messageMatchService ?? throw new ArgumentNullException(nameof(messageMatchService));
        _ = messageMatch ?? throw new ArgumentNullException(nameof(messageMatch));
        _ = message ?? throw new ArgumentNullException(nameof(message));

        return messageMatchService.IsMatch(
            messageMatch,
            message.GetType(),
            messageMatchService.GetMessageType(message));
    }
}