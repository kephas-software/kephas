// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistributedAppEventEmitter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the distributed application events emitter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Events
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Events;
    using Kephas.Services;

    /// <summary>
    /// An application events emitter service distributing the event over the whole application landscape.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DistributedAppEventEmitter : IAppEventEmitter
    {
        /// <summary>
        /// The message broker.
        /// </summary>
        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedAppEventEmitter"/> class.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        public DistributedAppEventEmitter(IMessageBroker messageBroker)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));

            this.messageBroker = messageBroker;
        }

        /// <summary>
        /// Asynchronously emits the provided event.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">Optional. the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task EmitAsync(IEvent appEvent, IContext context = null, CancellationToken cancellationToken = default)
        {
            return this.messageBroker.PublishAsync(appEvent, context, cancellationToken);
        }
    }
}