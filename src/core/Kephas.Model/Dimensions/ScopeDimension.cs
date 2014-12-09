// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dimension identifying an application scope.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    /// <summary>
    /// Dimension identifying an application scope. Application scopes may be domain models, client models, 
    /// application services and so on.
    /// </summary>
    public class ScopeDimension : AppDimensionBase
    {
        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The scope dimension is not aggregatable because the scope provides an orthogonal view over the application.
        /// </remarks>
        public override bool IsAggregatable
        {
            get { return false; }
        }
    }
}