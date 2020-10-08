// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A serializer metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// A serializer metadata.
    /// </summary>
    public class SerializerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public SerializerMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.MediaType = (Type)metadata.TryGetValue(nameof(this.MediaType), null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerMetadata" /> class.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">  The override priority.</param>
        public SerializerMetadata(Type mediaType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.MediaType = mediaType;
        }

        /// <summary>
        /// Gets the media type.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        public Type MediaType { get; }
    }
}