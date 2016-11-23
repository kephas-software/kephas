// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidatorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data validator metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation.Composition
{
    using System;
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// Metadata for data validator services.
    /// </summary>
    public class DataValidatorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidatorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataValidatorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.EntityType = (Type)metadata.TryGetValue(nameof(this.EntityType));
        }

        /// <summary>
        /// Gets the type of the entity being validated.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; }
    }
}