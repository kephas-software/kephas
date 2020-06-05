// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Runtime provider for model dimension information.
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
    /// Runtime factory for model dimension information.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class ModelDimensionConstructor : ModelElementConstructorBase<ModelDimension, IModelDimension, IRuntimeTypeInfo>
    {
        /// <summary>
        /// The dimension name discriminator.
        /// </summary>
        public const string DimensionNameDiscriminator = "Dimension";

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This discriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => DimensionNameDiscriminator;

        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return runtimeElement.TypeInfo.IsInterface;
        }

        /// <summary>
        /// Tries to get the model dimension information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override ModelDimension TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var typeInfo = runtimeElement.TypeInfo;
            var dimensionAttribute = typeInfo.GetCustomAttribute<ModelDimensionAttribute>();
            if (dimensionAttribute == null)
            {
                return null;
            }

            var modelElement = new ModelDimension(constructionContext, this.TryComputeNameCore(runtimeElement, constructionContext))
                                   {
                                       Index = dimensionAttribute.Index,
                                       IsAggregatable = dimensionAttribute.IsAggregatable
                                   };
            return modelElement;
        }
    }
}