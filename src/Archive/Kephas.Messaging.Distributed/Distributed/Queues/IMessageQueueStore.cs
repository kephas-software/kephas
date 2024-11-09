// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageQueueStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Queues;

using Kephas.Services;

/// <summary>
/// Singleton application service providing access to the message queues.
/// </summary>
[SingletonAppServiceContract]
public interface IMessageQueueStore
{
    /// <summary>
    /// Gets the message queue with the provided name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The message queue.</returns>
    IMessageQueue GetMessageQueue(string name);
}
