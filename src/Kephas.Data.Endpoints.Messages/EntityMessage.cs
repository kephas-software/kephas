// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    /// <summary>
    /// An entity message.
    /// </summary>
    public abstract class EntityMessage<TResponse> : IEntityMessage<TResponse>
    {
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public string? EntityType { get; set; }

        /// <summary>
        /// Gets or sets the entity reference, typically the ID.
        /// </summary>
        /// <value>
        /// The entity reference.
        /// </value>
        public object? EntityRef { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public object? Options { get; set; }
    }
}