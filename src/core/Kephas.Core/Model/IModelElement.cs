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
    public interface IModelElement
    {
        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        IEnumerable<IModelElement> Members { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IModelElement"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelElement))]
    internal abstract class ModelElementContractClass : IModelElement
    {
        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return "Name";
            }
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
                return new List<IModelElement>();
            }
        }
    }
}