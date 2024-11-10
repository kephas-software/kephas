// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageQueue.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Queues;

/// <summary>
/// Interface for a message queue.
/// </summary>
public interface IMessageQueue
{
    /// <summary>
    /// Occurs when a message arrives.
    /// </summary>
    public event AsyncEventHandler<MessageEventArgs> MessageArrived;

    /// <summary>
    /// Gets the channel name.
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Publishes the message asynchronously.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The asynchronous result.</returns>
    public Task PublishAsync(IBrokeredMessage message);
}
