// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementConfiguratorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Metadata for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Configuration.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// Metadata for element factories.
    /// </summary>
    public class ElementConfiguratorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementConfiguratorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ElementConfiguratorMetadata(IDictionary<string, object> metadata) 
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.NativeElementType = (Type)metadata.TryGetValue(nameof(this.NativeElementType));
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public Type NativeElementType { get; }
    }
}