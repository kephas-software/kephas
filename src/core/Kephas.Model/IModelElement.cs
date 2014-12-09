// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract providing base information about a model element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contract providing base information about a model element.
    /// </summary>
    [ContractClass(typeof(ModelElementContractClass))]
    public interface IModelElement : INamedElement
    {
        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        IModelProjection Projection { get; }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        IEnumerable<IModelElement> Members { get; }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        IEnumerable<IAttribute> Attributes { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IModelElement"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelElement))]
    internal abstract class ModelElementContractClass : IModelElement
    {
        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public IModelProjection Projection
        {
            get { return Contract.Result<IModelProjection>(); }
        }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public IEnumerable<IModelElement> Members
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IModelElement>>() != null);
                return Contract.Result<IEnumerable<IModelElement>>();
            }
        }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IAttribute>>() != null);
                return Contract.Result<IEnumerable<IAttribute>>();
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