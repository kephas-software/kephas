// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityForAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client model for attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute associating entities to their abstract model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EntityForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityForAttribute"/> class.
        /// </summary>
        /// <param name="modelType">The model type associated to the attributed type.</param>
        public EntityForAttribute(Type modelType)
        {
            this.ModelType = modelType;
        }

        /// <summary>
        /// Gets the model type associated to the attributed type.
        /// </summary>
        /// <value>
        /// The type of the model.
        /// </value>
        public Type ModelType { get; }
    }
}