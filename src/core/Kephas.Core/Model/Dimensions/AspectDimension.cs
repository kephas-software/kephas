// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dimension identifying orthogonal aspects within the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    /// <summary>
    /// Dimension identifying orthogonal aspects within the application. 
    /// By default, Kephas provide a default dimension element named 'Main',
    /// identifying the main concern of a model element.
    /// </summary>
    public class AspectDimension : AppDimensionBase
    {
        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The aspect dimension is aggregatable because it may provide logical aspects
        /// of the same element, which, after aggregation, are weaved into one integral element.
        /// </remarks>
        public override bool IsAggregatable
        {
            get { return true; }
        }
    }
}