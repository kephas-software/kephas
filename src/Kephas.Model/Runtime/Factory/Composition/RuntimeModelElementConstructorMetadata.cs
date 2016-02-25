// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementConstructorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Metadata for model element constructors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Metadata for model element constructors.
    /// </summary>
    public class RuntimeModelElementConstructorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The element info type metadata key.
        /// </summary>
        public static readonly string ModelTypeKey = ReflectionHelper.GetPropertyName<RuntimeModelElementConstructorMetadata>(m => m.ModelType);

        /// <summary>
        /// The runtime info type metadata key.
        /// </summary>
        public static readonly string RuntimeTypeKey = ReflectionHelper.GetPropertyName<RuntimeModelElementConstructorMetadata>(m => m.RuntimeType);

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementConstructorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RuntimeModelElementConstructorMetadata(IDictionary<string, object> metadata) 
            : base(metadata)
        {
            this.ModelType = (Type)metadata.TryGetValue(ModelTypeKey);
            this.RuntimeType = (Type)metadata.TryGetValue(RuntimeTypeKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementConstructorMetadata"/> class.
        /// </summary>
        /// <param name="modelType">Type of the element information.</param>
        /// <param name="runtimeType">Type of the runtime information.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">The override priority.</param>
        public RuntimeModelElementConstructorMetadata(Type modelType, Type runtimeType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.ModelType = modelType;
            this.RuntimeType = runtimeType;
        }

        /// <summary>
        /// Gets the type of the element information.
        /// </summary>
        /// <value>
        /// The type of the element information.
        /// </value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the type of the runtime information.
        /// </summary>
        /// <value>
        /// The type of the runtime information.
        /// </value>
        public Type RuntimeType { get; private set; }
    }
}