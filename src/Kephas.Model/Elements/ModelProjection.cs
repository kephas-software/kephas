// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelProjection.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the model projection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;

    using Kephas.Model.Construction;

    /// <summary>
    /// A model projection.
    /// </summary>
    public class ModelProjection : NamedElementBase<IModelProjection>, IModelProjection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelProjection" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        public ModelProjection(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
            this.Annotations = new List<IAnnotation>();
        }

        /// <summary>
        /// Gets the annotations of this model element.
        /// </summary>
        /// <value>
        /// The model element annotations.
        /// </value>
        public override IEnumerable<IAnnotation> Annotations { get; }

        /// <summary>
        /// Gets a value indicating whether this projection is the result of aggregating one or more projections.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is aggregated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAggregated { get; internal set; }

        /// <summary>
        /// Gets the dimension elements making up this projection.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        public IModelDimensionElement[] DimensionElements { get; internal set; }
    }
}