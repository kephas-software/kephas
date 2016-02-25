// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime provider for model dimension information.
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
    /// Runtime factory for model dimension information.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class ModelDimensionConstructor : ModelElementConstructorBase<ModelDimension, IDynamicTypeInfo>
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
        /// This dicriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => DimensionNameDiscriminator;

        /// <summary>
        /// Tries to get the model dimension information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override ModelDimension TryCreateModelElementCore(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
        {
            var typeInfo = runtimeElement.TypeInfo;
            if (!typeInfo.IsInterface)
            {
                return null;
            }

            var dimensionAttribute = typeInfo.GetCustomAttribute<ModelDimensionAttribute>();
            if (dimensionAttribute == null)
            {
                return null;
            }

            var modelElement = new ModelDimension(constructionContext, this.ComputeName(runtimeElement))
                                   {
                                       Index = dimensionAttribute.Index,
                                       IsAggregatable = dimensionAttribute.IsAggregatable
                                   };
            return modelElement;
        }
    }
}