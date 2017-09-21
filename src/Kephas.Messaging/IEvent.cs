// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvent.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    /// <summary>
    /// Marker interface for events, in a publisher/subscriber scenario.
    /// </summary>
    public interface IEvent : IMessage
    {
    }
}