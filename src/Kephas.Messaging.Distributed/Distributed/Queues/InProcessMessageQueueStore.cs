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
[OverridePriority(Priority.Low)]
public class InProcessMessageQueueStore : IMessageQueueStore
{
    private readonly ConcurrentDictionary<string, IMessageQueue> channels = new ();
    private readonly IContextFactory contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="InProcessMessageQueueStore"/> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <exception cref="System.ArgumentNullException">contextFactory</exception>
    public InProcessMessageQueueStore(IContextFactory contextFactory)
    {
        this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
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
        return new InProcessMessageQueue(this.contextFactory, name);
    }

    private class InProcessMessageQueue : Loggable, IMessageQueue
    {
        private readonly ConcurrentQueue<IBrokeredMessage> internalQueue = new ();

        public InProcessMessageQueue(IContextFactory contextFactory, string channel)
            : base(contextFactory)
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
                    this.Logger.Error(ex, $"Error while issuing the {nameof(this.MessageArrived)} event for message {{message}}.", msg);
                }
            });

            return Task.CompletedTask;
        }
    }
}
