// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatableModelDimensionAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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