﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityAssemblyAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity assembly attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    using System;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Marks an assembly as exporting <see cref="IEntityType"/> model elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class EntityAssemblyAttribute : ModelAssemblyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAssemblyAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// Using the default constructor indicates that all the types in the assembly should be
        /// considered entities.
        /// </remarks>
        public EntityAssemblyAttribute()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAssemblyAttribute" /> class.
        /// </summary>
        /// <remarks>
        /// Using the namespaces constructor indicates that only the assembly types in the indicates
        /// namespaces should be considered entities.
        /// </remarks>
        /// <param name="modelNamespaces">The namespaces containing model elements.</param>
        public EntityAssemblyAttribute(params string[] modelNamespaces)
            : base(modelNamespaces)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAssemblyAttribute" /> class.
        /// </summary>
        /// <remarks>
        /// Using the types constructor indicates that only the indicates assembly types should be
        /// considered entities.
        /// </remarks>
        /// <param name="modelTypes">The model types.</param>
        public EntityAssemblyAttribute(params Type[] modelTypes)
            : base(modelTypes)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes this object.
        /// </summary>
        private void Initialize()
        {
            this.DefaultClassifierKindAttribute = new EntityTypeAttribute();
        }
    }
}