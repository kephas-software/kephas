// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelProjection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for model projections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    /// <summary>
    /// Contract for model projections.
    /// </summary>
    public interface IModelProjection : INamedElement
    {
        /// <summary>
        /// Gets a value indicating whether this projection is the result of aggregating one or more projections.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is aggregated; otherwise, <c>false</c>.
        /// </value>
        bool IsAggregated { get; }

        /// <summary>
        /// Gets the name of the aggregated projection.
        /// </summary>
        /// <value>
        /// The name of the aggregated projection.
        /// </value>
        string AggregatedProjectionName { get; }

        /// <summary>
        /// Gets the aggregated projection, if the projection is part of an aggregated one.
        /// </summary>
        /// <value>
        /// The aggregated projection.
        /// </value>
        IModelProjection AggregatedProjection { get; }

        /// <summary>
        /// Gets the dimension elements making up this projection.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        IModelDimensionElement[] DimensionElements { get; }
    }
}