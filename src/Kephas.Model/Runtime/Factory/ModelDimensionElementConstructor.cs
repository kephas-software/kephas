﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime provider for model dimension element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;
    using Kephas.Services;

    /// <summary>
    /// Runtime provider for model dimension element information.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class ModelDimensionElementConstructor : ModelElementConstructorBase<ModelDimensionElement, IDynamicTypeInfo>
    {
        /// <summary>
        /// The dimension element name discriminator.
        /// </summary>
        public const string DimensionElementNameDiscriminator = "DimensionElement";

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This dicriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => DimensionElementNameDiscriminator;

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="dimensionName">Name of the dimension.</param>
        /// <returns>
        /// The element name, or <c>null</c> if the name could not be computed.
        /// </returns>
        protected virtual string ComputeName(object runtimeElement, string dimensionName)
        {
            var baseName = base.ComputeName(runtimeElement);
            if (baseName.EndsWith(dimensionName))
            {
                return baseName.Substring(0, baseName.Length - dimensionName.Length);
            }

            return baseName;
        }

        /// <summary>
        /// Tries to get the model dimension element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override ModelDimensionElement TryCreateModelElementCore(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
        {
            var typeInfo = runtimeElement.TypeInfo;
            if (!typeInfo.IsInterface)
            {
                return null;
            }

            var dimensionElementAttribute = typeInfo.GetCustomAttribute<ModelDimensionElementAttribute>();
            if (dimensionElementAttribute == null)
            {
                return null;
            }

            var dimensionName = this.ComputeDimensionName(typeInfo.Namespace);
            var modelElement = new ModelDimensionElement(constructionContext, this.ComputeName(runtimeElement, dimensionName))
                                        {
                                            DimensionName = dimensionName
                                        };
            return modelElement;
        }

        /// <summary>
        /// Computes the name of the parent dimension based on the runtime element namespace.
        /// </summary>
        /// <param name="ns">The namenspace.</param>
        /// <returns>The dimension name.</returns>
        protected virtual string ComputeDimensionName(string ns)
        {
            var lastNsPartIndex = ns.LastIndexOf('.');
            if (lastNsPartIndex > 0)
            {
                return ns.Substring(lastNsPartIndex + 1);
            }

            return ns;
        }
    }
}