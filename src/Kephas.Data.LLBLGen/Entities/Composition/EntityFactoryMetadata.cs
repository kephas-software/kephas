// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityFactoryMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity factory metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for <see cref="IEntityFactory"/> service.
    /// </summary>
    public class EntityFactoryMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFactoryMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public EntityFactoryMetadata(IDictionary<string, object?> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.EntityType = (Type)metadata.TryGetValue(nameof(this.EntityType));
        }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; set; }
    }
}