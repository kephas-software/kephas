// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for <see cref="IDataBehavior"/> services.
    /// </summary>
    public class DataBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataBehaviorMetadata(IDictionary<string, object?> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.EntityType = (Type)metadata.TryGetValue(nameof(this.EntityType));
        }

        /// <summary>
        /// Gets the type of the entity on which the behavior is applied.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; }
    }
}