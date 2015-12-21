// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of model dimension elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Implementation of model dimension elements.
    /// </summary>
    public class ModelDimensionElement : ModelElementBase<IModelDimensionElement, IModelDimensionElementInfo>, IModelDimensionElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionElement"/> class.
        /// </summary>
        /// <param name="elementInfo">The element information.</param>
        /// <param name="modelSpace">The model space.</param>
        public ModelDimensionElement(IModelDimensionElementInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
        {
        }
    }
}