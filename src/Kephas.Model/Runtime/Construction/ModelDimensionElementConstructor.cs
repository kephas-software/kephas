// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Runtime provider for model dimension element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Runtime provider for model dimension element information.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class ModelDimensionElementConstructor : ModelElementConstructorBase<ModelDimensionElement, IModelDimensionElement, IRuntimeTypeInfo>
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
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string TryComputeNameCore(object runtimeElement)
        {
            var typeInfo = (runtimeElement as IRuntimeTypeInfo)?.TypeInfo;
            var dimensionName = this.ComputeDimensionName(typeInfo?.Namespace);

            var baseName = base.TryComputeNameCore(runtimeElement);
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
        protected override ModelDimensionElement TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
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

            var modelElement = new ModelDimensionElement(constructionContext, this.TryComputeNameCore(runtimeElement))
                                        {
                                            DimensionName = this.ComputeDimensionName(typeInfo.Namespace)
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