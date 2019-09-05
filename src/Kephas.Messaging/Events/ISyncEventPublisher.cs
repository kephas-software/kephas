// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncAppEventEmitter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISyncAppEventEmitter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using Kephas.Services;

    /// <summary>
    /// Contract for the service which publishes events synchronously.
    /// </summary>
    public interface ISyncEventPublisher
    {
        /// <summary>
        /// Emits the provided event.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="context">Optional. the context.</param>
        void Publish(object @event, IContext context = null);
    }
}