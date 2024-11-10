// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelDimension.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines a model dimension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Defines a model dimension.
    /// </summary>
    [MemberNameDiscriminator("^")]
    public interface IModelDimension : IModelElement
    {
        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// A dimension is aggregatable if its members contains parts of an element which at runtime will be
        /// aggregated into one integral element. For example, this helps modeling application layers or aspects 
        /// which provide different logical views on the same element.
        /// </remarks>
        bool IsAggregatable { get; }

        /// <summary>
        /// Gets the dimension elements.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        IEnumerable<IModelDimensionElement> Elements { get; }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        int Index { get; }
    }
}