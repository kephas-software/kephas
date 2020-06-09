// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementConfiguratorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Metadata for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for element factories.
    /// </summary>
    public class RuntimeModelElementConfiguratorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementConfiguratorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RuntimeModelElementConfiguratorMetadata(IDictionary<string, object?> metadata) 
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ElementType = (Type)metadata.TryGetValue(nameof(this.ElementType));
            this.RuntimeElementType = (Type)metadata.TryGetValue(nameof(this.RuntimeElementType));
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public Type ElementType { get; }

        /// <summary>
        /// Gets the type of the runtime element.
        /// </summary>
        /// <value>
        /// The type of the runtime element.
        /// </value>
        public Type RuntimeElementType { get; }
    }
}