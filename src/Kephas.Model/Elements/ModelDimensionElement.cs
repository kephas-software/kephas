// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implementation of model dimension elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Construction;

    /// <summary>
    /// Implementation of model dimension elements.
    /// </summary>
    public class ModelDimensionElement : ModelElementBase<IModelDimensionElement>, IModelDimensionElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionElement"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public ModelDimensionElement(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the name of the dimension.
        /// </summary>
        /// <value>
        /// The name of the dimension.
        /// </value>
        public string DimensionName { get; internal set; }

        /// <summary>
        /// Gets the dimension declaring this element.
        /// </summary>
        /// <value>
        /// The declaring dimension.
        /// </value>
        public IModelDimension DeclaringDimension => (IModelDimension)this.DeclaringContainer;
    }
}