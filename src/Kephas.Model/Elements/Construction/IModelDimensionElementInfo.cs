// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelDimensionElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information for constructing model dimensions elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    /// <summary>
    /// Information for constructing model dimensions elements.
    /// </summary>
    public interface IModelDimensionElementInfo : IModelElementInfo
    {
        /// <summary>
        /// Gets the name of the parent dimension.
        /// </summary>
        /// <value>
        /// The name of the parent dimension.
        /// </value>
        string DimensionName { get; }
    }
}