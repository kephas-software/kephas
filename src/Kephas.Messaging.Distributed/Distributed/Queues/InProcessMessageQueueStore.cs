// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageQueueStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Queues;

using System.Collections.Concurrent;
using Kephas.Logging;
using Kephas.Services;

/// <summary>
/// Message queue store in process.
/// </summary>
/// <seealso cref="IMessageQueueStore" />
[Priority(Priority.Low)]
public class InProcessMessageQueueStore : IMessageQueueStore
{
    private readonly ConcurrentDictionary<string, IMessageQueue> channels = new ();
    private readonly IServiceFactory serviceFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="InProcessMessageQueueStore"/> class.
    /// </summary>
    /// <param name="serviceFactory">The injectable factory.</param>
    /// <exception cref="System.ArgumentNullException">contextFactory</exception>
    public InProcessMessageQueueStore(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    /// <summary>
    /// Gets the message queue with the provided name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>
    /// The message queue.
    /// </returns>
    public virtual IMessageQueue GetMessageQueue(string name)
    {
        return this.channels.GetOrAdd(name, _ => this.CreateMessageQueue(name));
    }

    /// <summary>
    /// Creates a message queue.
    /// </summary>
    /// <param name="name">The channel name.</param>
    /// <returns>The newly created message queue.</returns>
    protected IMessageQueue CreateMessageQueue(string name)
    {
        return new InProcessMessageQueue(this.serviceFactory, name);
    }

    private class InProcessMessageQueue : Loggable, IMessageQueue
    {
        private readonly ConcurrentQueue<IBrokeredMessage> internalQueue = new ();

        public InProcessMessageQueue(IServiceFactory serviceFactory, string channel)
            : base(serviceFactory)
        {
            this.Channel = channel;
        }

        public event AsyncEventHandler<MessageEventArgs> MessageArrived;

        public string Channel { get; }

        public Task PublishAsync(IBrokeredMessage message)
        {
            this.internalQueue.Enqueue(message);
            Task.Run(() =>
            {
                if (!this.internalQueue.TryDequeue(out var msg))
                {
                    return;
                }

                try
                {
                    this.MessageArrived?.Invoke(this, new MessageEventArgs(msg));
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"Error while issuing the {nameof(this.MessageArrived)} event for message {{message}}.", msg);
                }
            });

            return Task.CompletedTask;
        }
    }
}
