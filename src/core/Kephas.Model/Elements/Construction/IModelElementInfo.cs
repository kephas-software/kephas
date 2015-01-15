// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information for constructing model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Information for constructing model elements.
    /// </summary>
    [ContractClass(typeof(ModelElementInfoContractClass))]
    public interface IModelElementInfo : INamedElementInfo
    {
        /// <summary>
        /// Gets the members' constructor information.
        /// </summary>
        /// <value>
        /// The members' constructor information.
        /// </value>
        IEnumerable<INamedElementInfo> Members { get; } 
    }

    /// <summary>
    /// Contract class for <see cref="IModelElementInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelElementInfo))]
    internal abstract class ModelElementInfoContractClass : IModelElementInfo
    {
        /// <summary>
        /// Gets the members' constructor information.
        /// </summary>
        /// <value>
        /// The members' constructor information.
        /// </value>
        public IEnumerable<INamedElementInfo> Members
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<INamedElementInfo>>() != null);
                return Contract.Result<IEnumerable<INamedElementInfo>>();
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the function used to select the container.
        /// </summary>
        /// <value>
        /// The function used to select the container.
        /// </value>
        /// <remarks>
        /// This function returns <c>true</c> if the current element is member of the provided container.
        /// </remarks>
        public abstract Func<IModelElement, bool> IsMemberOf { get; }
    }
}