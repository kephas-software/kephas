// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoFactoryMetadata.cs" company="Quartz Software SRL">
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

    using Kephas.Extensions;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Metadata for element factories.
    /// </summary>
    public class RuntimeElementInfoFactoryMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The element info type metadata key.
        /// </summary>
        public static readonly string ElementInfoTypeKey = ReflectionHelper.GetPropertyName<RuntimeElementInfoFactoryMetadata>(m => m.ElementInfoType);

        /// <summary>
        /// The runtime info type metadata key.
        /// </summary>
        public static readonly string RuntimeInfoTypeKey = ReflectionHelper.GetPropertyName<RuntimeElementInfoFactoryMetadata>(m => m.RuntimeInfoType);

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeElementInfoFactoryMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RuntimeElementInfoFactoryMetadata(IDictionary<string, object> metadata) 
            : base(metadata)
        {
            this.ElementInfoType = (Type)metadata.TryGetValue(ElementInfoTypeKey, null);
            this.RuntimeInfoType = (Type)metadata.TryGetValue(RuntimeInfoTypeKey, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeElementInfoFactoryMetadata"/> class.
        /// </summary>
        /// <param name="elementInfoType">Type of the element information.</param>
        /// <param name="runtimeInfoType">Type of the runtime information.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">The override priority.</param>
        public RuntimeElementInfoFactoryMetadata(Type elementInfoType, Type runtimeInfoType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.ElementInfoType = elementInfoType;
            this.RuntimeInfoType = runtimeInfoType;
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