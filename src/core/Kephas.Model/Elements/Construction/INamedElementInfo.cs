// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information for constructing named elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Information for constructing named elements.
    /// </summary>
    [ContractClass(typeof(NamedElementInfoContractClass))]
    public interface INamedElementInfo
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the function used to select the container.
        /// </summary>
        /// <value>
        /// The function used to select the container.
        /// </value>
        /// <remarks>
        /// This function returns <c>true</c> if the current element is member of the provided container.
        /// </remarks>
        Func<IModelElement, bool> IsMemberOf { get; }
    }

    /// <summary>
    /// Contract class for <see cref="INamedElementInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(INamedElementInfo))]
    internal abstract class NamedElementInfoContractClass : INamedElementInfo
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
                return Contract.Result<string>();
            }
        }

        /// <summary>
        /// Gets or sets the function used to select the container.
        /// </summary>
        /// <value>
        /// The function used to select the container.
        /// </value>
        /// <remarks>
        /// This function returns <c>true</c> if the current element is member of the provided container.
        /// </remarks>
        public Func<IModelElement, bool> IsMemberOf { get; set; }
    }
}