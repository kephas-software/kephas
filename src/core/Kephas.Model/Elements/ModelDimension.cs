// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of model dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implementation of model dimensions.
    /// </summary>
    public class ModelDimension : ModelElementBase<IModelDimension>, IModelDimension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimension" /> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="name">The element name.</param>
        /// <param name="index">The dimension index.</param>
        /// <param name="isAggregatable">If set to <c>true</c>, the model dimension is aggregatable.</param>
        public ModelDimension(IModelSpace modelSpace, string name, int index, bool isAggregatable)
            : base(modelSpace, name)
        {
            this.Index = index;
            this.IsAggregatable = isAggregatable;
        }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        public int Index { get; private set; }

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
        public bool IsAggregatable { get; private set; }

        /// <summary>
        /// Gets the dimension elements.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        public IEnumerable<IModelDimensionElement> Elements
        {
            get { return this.Members.OfType<IModelDimensionElement>(); }
        }
    }
}