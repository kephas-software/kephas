// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of model dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Factory;

    /// <summary>
    /// Implementation of model dimensions.
    /// </summary>
    public class ModelDimension : ModelElementBase<IModelDimension>, IModelDimension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimension" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public ModelDimension(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// A dimension is aggregatable if its members contains parts of an element which at runtime will be
        /// aggregated into one integral element. For example, this helps modelling aplication layers or aspects
        /// which provide different logical views on the same element.
        /// </remarks>
        public bool IsAggregatable { get; internal set; }

        /// <summary>
        /// Gets the dimension elements.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        public IEnumerable<IModelDimensionElement> Elements => this.Members.OfType<IModelDimensionElement>();
    }
}