// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dimension identifying modules within the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    /// <summary>
    /// Dimension identifying modules within the application. Modules are functional units of an application which are autonomous
    /// but at the same time possibly integrated with one another.
    /// </summary>
    public class ModuleDimension : AppDimensionBase
    {
        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The module dimension is not aggregatable because it splits the application into
        /// autonomous units, which can interact with one another but which cannot be aggregated.
        /// </remarks>
        public override bool IsAggregatable
        {
            get { return false; }
        }
    }
}