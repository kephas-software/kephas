// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementConfiguratorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Metadata for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Metadata for element factories.
    /// </summary>
    public class ElementConfiguratorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The native element type metadata key.
        /// </summary>
        public static readonly string NativeElementTypeKey = ReflectionHelper.GetPropertyName<ElementConfiguratorMetadata>(m => m.NativeElementType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementConfiguratorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ElementConfiguratorMetadata(IDictionary<string, object> metadata) 
            : base(metadata)
        {
            object value;
            if (metadata.TryGetValue(NativeElementTypeKey, out value))
            {
                this.NativeElementType = (Type)value;
            }
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public Type NativeElementType { get; private set; }
    }
}