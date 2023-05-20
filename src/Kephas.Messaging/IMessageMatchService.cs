// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageMatchService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageMatchService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Messaging;

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
    /// <param name="context">The messaging context.</param>
    /// <returns>
    /// True if the message type and ID matches the criteria, false if not.
    /// </returns>
    bool IsMatch(IMessageMatch messageMatch, IMessagingContext context);
}