// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedEvent.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamedEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    /// <summary>
    /// Interface for named event.
    /// </summary>
    /// <remarks>
    /// Named events do not have to implement <see cref="INamedEvent"/>,
    /// it is sufficient to provide a string property by the name <see cref="INamedMessage.MessageName"/>.
    /// </remarks>
    public interface INamedEvent : IEvent, INamedMessage
    {
    }
}