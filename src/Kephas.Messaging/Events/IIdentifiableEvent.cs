// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifiableEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IIdentifiableEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using Kephas.Data;

    /// <summary>
    /// Interface for identifiable events.
    /// </summary>
    public interface IIdentifiableEvent : IEvent, IIdentifiable
    {
    }
}