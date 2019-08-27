// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeJsonSerializerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type JSON serializer metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// A type JSON serializer metadata.
    /// </summary>
    public class TypeJsonSerializerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeJsonSerializerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public TypeJsonSerializerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ValueType = (Type)metadata.TryGetValue(nameof(this.ValueType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeJsonSerializerMetadata"/> class.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public TypeJsonSerializerMetadata(Type valueType, int processingPriority = 0, int overridePriority = 0, string serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            Requires.NotNull(valueType, nameof(valueType));

            this.ValueType = valueType;
        }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        public Type ValueType { get; set; }
    }
}