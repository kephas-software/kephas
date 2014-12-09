// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a model dimension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Kephas.Model
{
    /// <summary>
    /// Defines a model dimension.
    /// </summary>
    [ContractClass(typeof (ModelDimensionContractClass))]
    public interface IModelDimension : INamedElement
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

    /// <summary>
    /// Contract class for <see cref="IModelDimension"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelDimension))]
    internal abstract class ModelDimensionContractClass : IModelDimension
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
        public bool IsAggregatable
        {
            get { return Contract.Result<bool>(); }
        }

        /// <summary>
        /// Gets the dimension elements.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        public IEnumerable<IModelDimensionElement> Elements
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IModelDimensionElement>>() != null);
                return Contract.Result<IEnumerable<IModelDimensionElement>>();
            }
        }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        public int Index
        {
            get
            {
                return Contract.Result<int>();
            }
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public abstract string Name { get; }
    }
}