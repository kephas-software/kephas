// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    /// <summary>
    /// Marker interface for events, in a publisher/subscriber scenario.
    /// </summary>
    public interface IEvent : IMessage<object?>
    {
    }
}