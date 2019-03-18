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
    /// Contract for the service which synchronously emits events at application level.
    /// </summary>
    public interface ISyncAppEventEmitter
    {
        /// <summary>
        /// Emits the provided event.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">Optional. the context.</param>
        void Emit(IEvent appEvent, IContext context = null);
    }
}