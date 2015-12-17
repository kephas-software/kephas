// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A serializer metadata.
    /// </summary>
    public class SerializerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The format type metadata key.
        /// </summary>
        public static readonly string FormatTypeKey = ReflectionHelper.GetPropertyName<SerializerMetadata>(m => m.FormatType);

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public SerializerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.FormatType = (Type)metadata.TryGetValue(FormatTypeKey, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerMetadata" /> class.
        /// </summary>
        /// <param name="formatType">        Type of the format.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">  The override priority.</param>
        public SerializerMetadata(Type formatType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.FormatType = formatType;
        }

        /// <summary>
        /// Gets or sets the format to use.
        /// </summary>
        /// <value>
        /// The format type.
        /// </value>
        public Type FormatType { get; set; }
    }
}