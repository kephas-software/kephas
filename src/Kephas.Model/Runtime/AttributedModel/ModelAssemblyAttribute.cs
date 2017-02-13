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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Model.Reflection;
    using Kephas.Reflection;

    /// <summary>
    /// Marks an assembly as exporting model elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ModelAssemblyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// Using the default constructor indicates that all the types in the assembly should be
        /// considered model elements.
        /// </remarks>
        public ModelAssemblyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyAttribute" /> class.
        /// </summary>
        /// <remarks>
        /// Using the namespaces constructor indicates that only the assembly types in the indicates
        /// namespaces should be considered model elements.
        /// </remarks>
        /// <param name="modelNamespaces">The namespaces containing model elements.</param>
        public ModelAssemblyAttribute(params string[] modelNamespaces)
        {
            this.ModelNamespaces = modelNamespaces;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyAttribute" /> class.
        /// </summary>
        /// <remarks>
        /// Using the types constructor indicates that only the indicates assembly types should be
        /// considered model elements.
        /// </remarks>
        /// <param name="modelTypes">The model types.</param>
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
        public Type[] ModelTypes { get; }

        /// <summary>
        /// Gets the model namespaces.
        /// </summary>
        /// <value>
        /// The model namespaces.
        /// </value>
        public string[] ModelNamespaces { get; }

        /// <summary>
        /// Gets or sets the default classifier kind attribute for the provided namespaces and types.
        /// </summary>
        /// <value>
        /// The default classifier type.
        /// </value>
        public ClassifierKindAttribute DefaultClassifierKindAttribute { get; protected set; }

        /// <summary>
        /// Gets a filter for types' namespaces within the assembly.
        /// </summary>
        /// <param name="modelAssemblyAttributes">The model assembly attributes.</param>
        /// <returns>
        /// The model assembly namespace filter.
        /// </returns>
        protected internal static Func<Type, bool> GetModelAssemblyNamespaceFilter(IEnumerable<ModelAssemblyAttribute> modelAssemblyAttributes)
        {
            var allTypesAttribute = modelAssemblyAttributes.FirstOrDefault(a => a.ModelTypes == null && a.ModelNamespaces == null);
            if (allTypesAttribute != null)
            {
                // if no model types or namespaces are indicated, simply add all
                // exported types from the assembly with no further processing
                return null;
            }
            
            // add only the types from the provided namespaces
            var namespaces = new HashSet<string>(modelAssemblyAttributes.Where(a => a.ModelNamespaces != null && a.ModelNamespaces.Length > 0).SelectMany(a => a.ModelNamespaces));
            var namespacePatterns = namespaces.Select(n => n + ".").ToList();
            return t => namespaces.Contains(t.Namespace) || namespacePatterns.Any(p => t.Namespace.StartsWith(p));
        }
    }
}