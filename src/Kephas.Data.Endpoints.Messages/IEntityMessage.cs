// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using Kephas.Messaging;

    /// <summary>
    /// Contract for messages targeting a specific entity.
    /// </summary>
    public interface IEntityMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        string? EntityType { get; set; }

        /// <summary>
        /// Gets or sets the entity reference, typically the ID.
        /// </summary>
        /// <value>
        /// The entity reference.
        /// </value>
        object? EntityRef { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        object? Options { get; set; }
    }
}