// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelDimensionInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information for constructing model dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    /// <summary>
    /// Information for constructing model dimensions.
    /// </summary>
    public interface IModelDimensionInfo : IModelElementInfo
    {
        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        int Index { get; }

        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        bool IsAggregatable { get; }
    }
}