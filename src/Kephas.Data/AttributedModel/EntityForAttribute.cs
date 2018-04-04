// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityForAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client model for attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// Attribute associating entities to their abstract model.
    /// </summary>
    /// <remarks>
    /// This attribute is typically used to annotate the concrete implementation of an abstract entity
    /// defined either as an abstract class or as an interface. This association helps the infrastructure
    /// to find the concrete implementation based only on the abstract type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EntityForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityForAttribute"/> class.
        /// </summary>
        /// <param name="modelType">The model type associated to the attributed type.</param>
        public EntityForAttribute(Type modelType)
        {
            Requires.NotNull(modelType, nameof(modelType));

            this.ModelType = modelType;
            this.ModelTypeName = modelType.GetAssemblyQualifiedShortName();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityForAttribute"/> class.
        /// </summary>
        /// <param name="modelTypeName">The name of the model type associated to the attributed type.</param>
        public EntityForAttribute(string modelTypeName)
        {
            Requires.NotNullOrEmpty(modelTypeName, nameof(modelTypeName));

            this.ModelTypeName = modelTypeName;
        }

        /// <summary>
        /// Gets the model type associated to the attributed type.
        /// </summary>
        /// <value>
        /// The associated model type.
        /// </value>
        public Type ModelType { get; }

        /// <summary>
        /// Gets the name of the model type associated to the attributed type.
        /// </summary>
        /// <value>
        /// The name of the associated model type.
        /// </value>
        public string ModelTypeName { get; }
    }
}