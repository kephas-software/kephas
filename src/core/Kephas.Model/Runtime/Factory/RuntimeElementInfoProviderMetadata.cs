// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoProviderMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Metadata for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Metadata for element factories.
    /// </summary>
    public class RuntimeElementInfoProviderMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The element info type metadata key.
        /// </summary>
        public static readonly string ElementInfoTypeKey = ReflectionHelper.GetPropertyName<RuntimeElementInfoProviderMetadata>(m => m.ElementInfoType);

        /// <summary>
        /// The runtime info type metadata key.
        /// </summary>
        public static readonly string RuntimeInfoTypeKey = ReflectionHelper.GetPropertyName<RuntimeElementInfoProviderMetadata>(m => m.RuntimeInfoType);

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeElementInfoProviderMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RuntimeElementInfoProviderMetadata(IDictionary<string, object> metadata) 
            : base(metadata)
        {
            object value;
            if (metadata.TryGetValue(ElementInfoTypeKey, out value))
            {
                this.ElementInfoType = (Type)value;
            }

            if (metadata.TryGetValue(RuntimeInfoTypeKey, out value))
            {
                this.RuntimeInfoType = (Type)value;
            }
        }

        /// <summary>
        /// Gets the type of the element information.
        /// </summary>
        /// <value>
        /// The type of the element information.
        /// </value>
        public Type ElementInfoType { get; private set; }

        /// <summary>
        /// Gets the type of the runtime information.
        /// </summary>
        /// <value>
        /// The type of the runtime information.
        /// </value>
        public Type RuntimeInfoType { get; private set; }
    }
}