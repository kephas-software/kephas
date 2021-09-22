// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementConstructorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Metadata for model element constructors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System;
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Metadata for model element constructors.
    /// </summary>
    public class RuntimeModelElementConstructorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementConstructorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RuntimeModelElementConstructorMetadata(IDictionary<string, object?>? metadata) 
            : base(metadata)
        {
            this.ModelType = (Type)metadata.TryGetValue(nameof(this.ModelType));
            this.ModelContractType = (Type)metadata.TryGetValue(nameof(this.ModelContractType));
            this.RuntimeType = (Type)metadata.TryGetValue(nameof(this.RuntimeType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelElementConstructorMetadata"/> class.
        /// </summary>
        /// <param name="modelType">Type of the element information.</param>
        /// <param name="modelContractType">The type of the model contract.</param>
        /// <param name="runtimeType">Type of the runtime information.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">The override priority.</param>
        public RuntimeModelElementConstructorMetadata(Type modelType, Type modelContractType, Type runtimeType, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            Requires.NotNull(modelType, nameof(modelType));
            Requires.NotNull(modelContractType, nameof(modelContractType));
            Requires.NotNull(runtimeType, nameof(runtimeType));

            this.ModelType = modelType;
            this.ModelContractType = modelContractType;
            this.RuntimeType = runtimeType;
        }

        /// <summary>
        /// Gets the type of the concrete implementation.
        /// </summary>
        /// <value>
        /// The type of the concrete implementation.
        /// </value>
        public Type ModelType { get; }

        /// <summary>
        /// Gets the type of the model contract (the interface).
        /// </summary>
        /// <value>
        /// The type of the model contract.
        /// </value>
        public Type ModelContractType { get; }

        /// <summary>
        /// Gets the type of the runtime definition.
        /// </summary>
        /// <value>
        /// The type of the runtime definition.
        /// </value>
        public Type RuntimeType { get; }
    }
}