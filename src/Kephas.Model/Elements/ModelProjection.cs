// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelProjection.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model projection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
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
        /// <param name="aggregatedProjectionName">Name of the aggregated projection.</param>
        public ModelProjection(IModelConstructionContext constructionContext, string name, string aggregatedProjectionName)
            : base(constructionContext, name)
        {
            Requires.NotNull(aggregatedProjectionName, nameof(aggregatedProjectionName));

            this.Annotations = new List<IAnnotation>();
            this.AggregatedProjectionName = aggregatedProjectionName;
        }

        /// <summary>
        /// Gets the annotations of this model element.
        /// </summary>
        /// <value>
        /// The model element annotations.
        /// </value>
        public override IEnumerable<IAnnotation> Annotations { get; }

        /// <summary>
        /// Gets the name of the aggregated projection.
        /// </summary>
        /// <value>
        /// The name of the aggregated projection.
        /// </value>
        public string AggregatedProjectionName { get; }

        /// <summary>
        /// Gets the aggregated projection.
        /// </summary>
        /// <value>
        /// The aggregated projection.
        /// </value>
        public IModelProjection AggregatedProjection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this projection is the result of aggregating one or more projections.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is aggregated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAggregated => this.Name == this.AggregatedProjectionName;

        /// <summary>
        /// Gets the dimension elements building this projection.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        public IModelDimensionElement[] DimensionElements { get; internal set; }

        /// <summary>
        /// Adds a part to the aggregated element.
        /// </summary>
        /// <param name="part">The part to be added.</param>
        protected override void AddPart(object part)
        {
            base.AddPart(part);

            var partProjection = part as ModelProjection;
            if (partProjection != null)
            {
                partProjection.AggregatedProjection = this;
            }
        }
    }
}