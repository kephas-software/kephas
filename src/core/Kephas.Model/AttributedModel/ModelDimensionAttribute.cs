// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Attribute marking dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute marking dimensions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ModelDimensionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionAttribute" /> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public ModelDimensionAttribute(int index)
            : this(false, index)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionAttribute" /> class.
        /// </summary>
        /// <param name="isAggregatable">If set to <c>true</c> the dimension is aggregatable.</param>
        /// <param name="index">The index.</param>
        protected ModelDimensionAttribute(bool isAggregatable, int index)
        {
            this.IsAggregatable = isAggregatable;
            this.Index = index;
        }

        /// <summary>
        /// Gets a value indicating whether the dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        public bool IsAggregatable { get; private set; }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets or sets the default dimension element.
        /// </summary>
        /// <value>
        /// The default dimension element.
        /// </value>
        public Type DefaultDimensionElement { get; set; }
    }
}