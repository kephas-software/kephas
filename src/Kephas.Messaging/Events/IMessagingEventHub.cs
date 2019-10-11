// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessagingEventHub.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessagingEventHub interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Services;

    /// <summary>
    /// Interface for event hubs supporting messaging.
    /// </summary>
    public interface IMessagingEventHub
    {
        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="match">Specifies the match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        IEventSubscription Subscribe(IMessageMatch match, Func<object, IContext, CancellationToken, Task> callback);
    }
}
