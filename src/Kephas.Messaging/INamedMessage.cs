// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamedMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    /// <summary>
    /// Marker interface for named messages.
    /// </summary>
    /// <remarks>
    /// Named messages do not have to implement <see cref="INamedMessage"/>,
    /// it is sufficient to provide a string property by the name <see cref="MessageName"/>.
    /// </remarks>
    public interface INamedMessage : IMessage
    {
        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        /// <value>
        /// The name of the message.
        /// </value>
        string MessageName { get; }
    }
}