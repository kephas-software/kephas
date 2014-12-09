// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines an application dimension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    /// <summary>
    /// Defines an application dimension.
    /// </summary>
    public interface IAppDimension : IModelElement
    {
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
        bool IsAggregatable { get; }
    }
}