// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEventArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Queues;

/// <summary>
/// Event arguments providing the brokered message.
/// </summary>
public class MessageEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public MessageEventArgs(IBrokeredMessage message)
    {
        this.Message = message;
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public IBrokeredMessage Message { get; }
}