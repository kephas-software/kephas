// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;

    /// <summary>
    /// Interface for application service.
    /// </summary>
    public interface IAppServiceType : IClassifier
    {
        /// <summary>
        /// Gets a value indicating whether the service allows multiple service types.
        /// </summary>
        /// <value>
        /// True if allow multiple, false if not.
        /// </value>
        bool AllowMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the service is exported as an open generic.
        /// </summary>
        /// <value>
        /// True if the service is exported as an open generic, false if not.
        /// </value>
        bool AsOpenGeneric { get; }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        Type ContractType { get; }
    }
}