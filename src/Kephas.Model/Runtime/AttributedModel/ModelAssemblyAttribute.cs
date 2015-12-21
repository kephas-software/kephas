// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAssemblyAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks an assembly as exporting model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.AttributedModel
{
    using System;

    /// <summary>
    /// Marks an assembly as exporting model elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ModelAssemblyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyAttribute"/> class.
        /// </summary>
        /// <remarks>Using the default constructor indicates that all the types in the assembly should be considered model elements.</remarks>
        public ModelAssemblyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyAttribute" /> class.
        /// </summary>
        /// <param name="modelNamespaces">The namespaces containing model elements.</param>
        /// <remarks>
        /// Using the namespaces constructor indicates that only the assembly types in the indicates namespaces should be considered model elements.
        /// </remarks>
        public ModelAssemblyAttribute(params string[] modelNamespaces)
        {
            this.ModelNamespaces = modelNamespaces;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyAttribute" /> class.
        /// </summary>
        /// <param name="modelTypes">The model types.</param>
        /// <remarks>
        /// Using the types constructor indicates that only the indicates assembly types should be considered model elements.
        /// </remarks>
        public ModelAssemblyAttribute(params Type[] modelTypes)
        {
            this.ModelTypes = modelTypes;
        }

        /// <summary>
        /// Gets the model types.
        /// </summary>
        /// <value>
        /// The model types.
        /// </value>
        public Type[] ModelTypes { get; private set; }

        /// <summary>
        /// Gets the model namespaces.
        /// </summary>
        /// <value>
        /// The model namespaces.
        /// </value>
        public string[] ModelNamespaces { get; private set; }
    }
}