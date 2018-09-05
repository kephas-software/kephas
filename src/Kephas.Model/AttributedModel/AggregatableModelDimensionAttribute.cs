// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatableModelDimensionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aggregatable model dimension attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute for identifying aggregatable dimensions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AggregatableModelDimensionAttribute : ModelDimensionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatableModelDimensionAttribute" /> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public AggregatableModelDimensionAttribute(int index)
            : base(true, index)
        {
        }
    }
}